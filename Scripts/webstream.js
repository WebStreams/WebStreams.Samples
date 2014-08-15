; (function (global) {
    var init = function (context) {
        /**
        * Connect to the stream specified by path, returning an observable.
        * 
        * @method WebStream
        * @param {String} path The path of the WebSocket to connect to.
        *   Either an absolute path ("/stock/ticker") or a URI.
        * @param {Object} queryParams The query parameters to append to the path.
        * @param {Object} inputStreams The collection of observables to pipe into
        *   the stream. Note that keys must match those specified on the service.
        * @param {Object} controlEvents The optional Rx.Subject which receives
        *   control messages.
        * @return {Object} Returns an Rx.Observable which will connect to the 
        *   specified path when subscribed to, emitting all received messages.
        */
        context.WebStream = function (path, queryParams, inputStreams, controlEvents) {
            return Rx.Observable.create(function (observer) {
                var self = {};
                self.params = queryParams || {};
                self.subscriptions = [];
                self.inputs = inputStreams || {};

                // Convert path into a URL if it isn't already.
                if (/^\w+\:\/\//i.test(path)) {
                    self.url = path;
                } else {
                    // Use the current origin, replacing http with ws and https with wss.
                    var origin = window.location.origin;
                    self.url = origin.replace(/^http(\w)?\:\/\//i, 'ws$1://') + path;
                }

                // Add all parameters to the URL query string.
                if (self.params) {
                    self.url += '?' + toQueryString(self.params);

                    // Encode the given object as a query string.
                    function toQueryString(object) {
                        var parts = [];
                        for (var key in object) {
                            var value = object[key];
                            parts.push(encodeURIComponent(key) + '=' + encodeURIComponent(value));
                        }

                        return parts.join('&');
                    }
                }

                // Connect to the uri.
                self.socket = new WebSocket(self.url);

                self.isDisposed = false;
                // On disposal, close the socket & unsubscribe from all inputs.
                self.dispose = function () {

                    if (!self.isDisposed) {
                        // Unsubscribe from all inputs.
                        if (self.subscriptions) {
                            for (var i in self.subscriptions) {
                                self.subscriptions[i].dispose();
                            }
                            self.subscriptions = [];
                        }

                        // Close the socket.
                        self.socket.close();
                        if (controlEvents) {
                            controlEvents.onNext('disposed');
                        }

                        self.isDisposed = true;
                    }
                };

                self.socket.onopen = function() {
                    // Subscribe to each input, piping inputs to socket.
                    for (var i in self.inputs) {
                        self.subscriptions.push(self.inputs[i].subscribe(
                            function(next) {
                                if (self.isDisposed) return;
                                // Send a 'Next' event.
                                var nextMsg = '';
                                if (next !== undefined && next !== null) {
                                    nextMsg = JSON.stringify(next);
                                }

                                self.socket.send('n' + i + '.' + nextMsg);
                            },
                            function(error) {
                                if (self.isDisposed) return;
                                // Send an 'Error' event.
                                self.socket.send('e' + i + '.' + JSON.stringify(error));
                            },
                            function() {
                                if (self.isDisposed) return;
                                // Send a 'Completed' event.
                                self.socket.send('c' + i);
                            }));
                    }

                    if (controlEvents) {
                        controlEvents.onNext('connected');
                    }
                };

                // If the socket closes, complete the sequence and unsubscribe from all inputs.
                self.socket.onclose = function () {
                    if (self.isDisposed) return;
                    observer.onCompleted();
                    if (controlEvents) {
                        controlEvents.onNext('closed');
                    }
                    self.dispose();
                };

                // If the socket errors, propagate that error and unsubscribe from all inputs.
                self.socket.onerror = function (error) {
                    if (self.isDisposed) return;
                    if (controlEvents) {
                        controlEvents.onNext('error: ' + JSON.stringify(error));
                    }
                    observer.onError(error);
                    self.dispose();
                };

                // Each time a message is received, parse it and propagate the results to the observer.
                self.socket.onmessage = function (message) {
                    if (self.isDisposed) return;
                    if (message.data.length > 0) {
                        switch (message.data[0]) {
                            case 'n':
                                // Propagate 'Next' event.
                                observer.onNext(JSON.parse(message.data.slice(1)));
                                break;
                            case 'e':
                                // Propagate 'Error' event & dispose.
                                observer.onError(JSON.parse(message.data.slice(1)));
                                self.dispose();
                                break;
                            case 'c':
                                // Propagate 'Completed' event & dispose.
                                observer.onCompleted();
                                self.dispose();
                                break;
                        }
                    }
                };

                // Return the disposal method to the subscriber.
                return self.dispose;
            });
        };
    };

    if (typeof define === 'function' && define.amd) {
        define(['rx'], function () {
            return init({});
        })
    } else {
        init(global);
    }
}(this));