// <reference name="MicrosoftAjax.js"/>
Type.registerNamespace("SitefinityWebApp.ReorderDynamicContent");

SitefinityWebApp.ReorderDynamicContent.ReorderDialogCustom = function (element) {

    SitefinityWebApp.ReorderDynamicContent.ReorderDialogCustom.initializeBase(this, [element]);
};

SitefinityWebApp.ReorderDynamicContent.ReorderDialogCustom.prototype = {

    /* --------------------------------- set up and tear down --------------------------------- */

    initialize: function () {
        SitefinityWebApp.ReorderDynamicContent.ReorderDialogCustom.callBaseMethod(this, 'initialize');
    },

    dispose: function () {
        SitefinityWebApp.ReorderDynamicContent.ReorderDialogCustom.callBaseMethod(this, 'dispose');
    },

    prepareDialog: function (dataItem, params) {
        // no folder param
        params["folderId"] = "";
        // no parent call needed
        if (!dataItem.SystemParentType) {
            dataItem.Id = "";           
            parentId = "";
        } // set the parent id to take advantage of the default logic
        else {
            dataItem.ParentId = dataItem.SystemParentId;
        }

        // base logic
        SitefinityWebApp.ReorderDynamicContent.ReorderDialogCustom.callBaseMethod(this, 'prepareDialog', [dataItem, params]);
        // set the dialog title
        this._dialogTitle.innerHTML = String.format(this._dialogTitlePattern, params["itemNameWithArticle"] || "Items");
    },

    _saveChanges: function () {
        var result = [];
        var itemElements = this._imageBinder.get_currentItemElements();
        for (var i = 0, length = itemElements.length; i < length; i++) {
            var element = itemElements[i];
            var dataItem = jQuery(element).data("dataItem");
            var ordinal = dataItem.Ordinal;
            var initialOrdinal = jQuery(element).data("initialOrdinal");

            if (initialOrdinal != null && initialOrdinal != dataItem.Ordinal) {
                result.push({ Key: dataItem.Id, Value: dataItem.Ordinal });
            }
        }
        
        var clientManager = this._imageBinder.get_manager();
        var serviceUrl = this._webSaveServiceUrl + "/ReorderDynamicContent/";
        var urlParams = [];

        if (this._itemType != null) {
            urlParams["itemType"] = this._itemType;
        }

        if (this._providerName) {
            urlParams["provider"] = this._providerName;
        }

        var keys = [];
        var data = result;
        clientManager.InvokePut(serviceUrl, urlParams, keys, data, this._saveChangesSuccess, this._saveChangesFailure, this);
    }
};

SitefinityWebApp.ReorderDynamicContent.ReorderDialogCustom.registerClass('SitefinityWebApp.ReorderDynamicContent.ReorderDialogCustom',
    Telerik.Sitefinity.Modules.Libraries.Web.UI.ReorderDialog);