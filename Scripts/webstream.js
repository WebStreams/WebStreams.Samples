;(function(global) {
    var init = function(context) {
        context.WebStream = function(path, params, inputs) {
            return Rx.Observable.create(function(observer) {
                var self = {};
                self.params = params || {};
                self.subscriptions = [];
                self.inputs = inputs || {};

                self.url = window.location.origin.replace('http://', 'ws://') + path;
                if (params) {
                    self.url += '?' + $.param(params);
                }

                self.socket = new WebSocket(self.url);

                self.dispose = function () {
                    if (self.subscriptions) {
                        for (var i in self.subscriptions) {
                            self.subscriptions[i].dispose();
                        }
                        self.subscriptions = [];
                    }

                    self.socket.close();
                };

                self.socket.onclose = function () {
                    observer.onCompleted();
                    self.dispose();
                };

                self.socket.onerror = function (error) {
                    observer.onError(error);
                    self.dispose();
                };

                self.socket.onmessage = function (message) {
                    if (message.data.length > 0) {
                        switch (message.data[0]) {
                        case 'n':
                            // onNext
                            observer.onNext(JSON.parse(message.data.slice(1)));
                            break;
                        case 'e':
                            // onError
                            observer.onError(JSON.parse(message.data.slice(1)));
                            self.dispose();
                            break;
                        case 'c':
                            // onCompleted
                            observer.onCompleted();
                            self.dispose();
                            break;
                        }
                    }
                };

                // Subscribe to each input, piping inputs to socket.
                for (var i in self.inputs) {
                    self.subscriptions.push(self.inputs[i].subscribe(function(next) {
                            self.socket.send('n' + i + '.' + JSON.stringify(next));
                        },
                        function(error) {
                            self.socket.send('e' + i + '.' + JSON.stringify(error));
                        },
                        function() {
                            self.socket.send('c' + i);
                        }));
                }

                return self.dispose;
            });
        };
    };

    if (typeof define === 'function' && define.amd) {
        define(['rx'], function(){
            return init({});
        })
    } else {
        init(global);
    }
}(this));