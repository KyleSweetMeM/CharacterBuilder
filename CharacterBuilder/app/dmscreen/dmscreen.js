﻿define(function (require) {
    var _i = {
        ko: require('knockout'),
        $: require('jquery'),
        moment: require('moment'),
        charajax: require('_custom/services/WebAPI'),
        list: require('_custom/services/listmanager'),
        deferred: require('_custom/deferred'),
        alert: require('_custom/services/alert'),
        globals: require('_custom/services/builderglobals'),
        confirmdelete: require('confirmdelete/confirmdelete'),
        newcampaignprompt: require('newcampaignprompt/newcampaignprompt'),
        roller: require('_custom/services/roll'),
        abilityscore: require('_custom/services/abilityscore'),
        checklist: require('_custom/services/checklist')
    };

    return function () {
        var self = this;
        self.campaigns = _i.ko.observableArray([]);

        self.activate = function () {
            return self.getPageData().done(function () {

            });
        };

        self.deleteCampaign = function (obj) {
            _i.confirmdelete.show().then(function (response) {
                if (response.accepted) {
                    _i.charajax.delete('api/dungeonmaster/DeleteCampaign/' + obj.Id(), '').done(function (response) {
                        self.campaigns.remove(obj);
                        _i.alert.showAlert({ type: "error", message: "Campaign Deleted" });
                    });
                }
            });
        };

        self.getPageData = function () {
            var deferred = _i.deferred.create();
            var promise = _i.deferred.waitForAll(self.getCampaigns());

            promise.done(function () {
                deferred.resolve();
            });

            return deferred;
        };

        self.selectCampaign = function (campSelected) {
            _i.alert.showAlert({ type: "success", message: "Entering the realm of " + campSelected.Name() });
            window.location.href = '#dmplayercards/' + campSelected.Id();
        };

        self.addNew = function () {
            _i.newcampaignprompt.show().then(function (response) {
                _i.charajax.post('api/dungeonmaster/CreateCampaign/'+ response).done(function (response) {
                    _i.alert.showAlert({ type: "success", message: "Created new Campaign!" });

                    window.location.href = '#dmplayercards/' + response.Id;
                });
            });           
        };

        self.getCampaigns = function () {
            var deferred = _i.deferred.create();
            _i.charajax.get('api/dungeonmaster/Campaigns').done(function(response) {
                var mapped = _i.ko.mapping.fromJS(response);
                var newMap =  _i.ko.mapping.fromJS(response, {
                    create: function (options) {
                        return _i.ko.observable(options.data);
                    }
                });

                self.campaigns(newMap());

                deferred.resolve();
            });
            return deferred;
        };

        self.draggedPhoto = _i.ko.observable();

        self.handleDragStart = function (photo, e) {
            console.log(photo);
            self.draggedPhoto(photo);
            console.log('dragStart');
            // Returning true is required to stop KO squashing the default action
            // This will allow dragover to take over from dragstart
            return true;
        }.bind(self);

        self.handleDragOver = function (e) {
            console.log('dragOver');
        }.bind(self);

        self.handleDrop = function (photo, e) {
            console.log('drop');
            // The next 3 lines shows how you can copy the dragged photo onto the dropped target...
            var context = ko.contextFor(e.target);
            var index = context.$index();
            self.campaigns()[index](self.draggedPhoto());
        }.bind(self);
    }
});
