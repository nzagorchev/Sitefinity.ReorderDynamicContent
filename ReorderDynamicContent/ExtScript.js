// called by the MasterGridView when it is loaded
function OnModuleMasterViewLoadedCustom(sender, args) {
    this._masterView = sender;

    this._itemsGrid = this._masterView.get_itemsGrid();

    this._masterBeforeCommandDelegate = Function.createDelegate(this, _masterBeforeCommandHandlerCustom);
    this._itemsGrid.add_beforeCommand(this._masterBeforeCommandDelegate);

}

function _masterBeforeCommandHandlerCustom(sender, args) {
    var commandName = args.get_commandName();
    var binder = this._masterView.get_currentItemsList().getBinder();
    var selectedItems = binder.get_selectedItems();
    var selectedItem = null;

    if (commandName == "reorder") {
        sender.openDialog('reorder', args.get_commandArgument(), sender._getDialogParameters('reorder'), null);
    }
}