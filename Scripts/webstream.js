;(function(global) {
    var init = function(context) {
        context.WebStream = function (path, params, inputs) {
                var self = this;
                self.params = params || {};
                self.subscriptions = [];
                self.inputs = inputs || {};

                var subject = new Rx.Subject();
                var url = window.location.origin.replace('http://', 'ws://') + path;
                if (params) {
                    url += '?' + $.param(params);
                }

                self.socket = new WebSocket(url);

                socket.onclose = function() {
                    subject.onCompleted();
                    dispose(self.subscriptions);
                };

                socket.onerror = function(error) {
                    subject.onError(error);
                    dispose(self.subscriptions);
                };

                socket.onmessage = function(message) {
                    if (message.data.length > 0) {
                        switch (message.data[0]) {
                        case 'n':
                            // onNext
                            subject.onNext(JSON.parse(message.data.slice(1)));
                            break;
                        case 'e':
                            // onError
                            subject.onError(JSON.parse(message.data.slice(1)));
                            dispose(self.subscriptions);
                            break;
                        case 'c':
                            // onCompleted
                            subject.onCompleted();
                            dispose(self.subscriptions);
                            break;
                        }
                    }
                };

                self.subscriptions = subscribe(self.socket, self.inputs);

                function subscribe(socket, inputs) {
                    var subscriptions = [];
                    for (var i in inputs) {
                        var subscription = subscribeOne(inputs[i]);
                        subscriptions.push(subscription);
                    }

                    return subscriptions;

                    function subscribeOne(input){
                        return input.subscribe(onNext, onError, onCompleted);
                        function onNext(next) {
                            socket.send('n' + i + '.' + JSON.stringify(next));
                        }
                        function onError(error) {
                            socket.send('e' + i + '.' + JSON.stringify(error));
                        }
                        function onCompleted() {
                            socket.send('c' + i);
                        }
                    }
                };

                function dispose(subscriptions) {
                    for (var i in subscriptions) {
                        subscriptions[i].dispose();
                    }
                    self.subscriptions = [];
                };

                subject.url = url;
                subject.inputs = inputs;
                subject.params = params;

                return subject;
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