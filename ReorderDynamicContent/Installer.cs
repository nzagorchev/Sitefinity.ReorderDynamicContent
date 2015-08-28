using System;
using System.Linq;
using System.Web.UI;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Web.UI;
using Telerik.Sitefinity.Web.UI.Backend.Elements.Widgets;
using Telerik.Sitefinity.Web.UI.ContentUI.Config;
using Telerik.Sitefinity.Web.UI.ContentUI.Views.Backend.Master.Config;

namespace SitefinityWebApp.ReorderDynamicContent
{
    public class Installer
    {
        public static void InstallInfrastructure()
        {
            Bootstrapper.Initialized += new EventHandler<ExecutedEventArgs>(RegisterService);
            ObjectFactory.Initializing += new EventHandler<ExecutingEventArgs>(RegisterDialog);
        }

        public static void RegisterService(object sender, ExecutedEventArgs e)
        {
            if (e.CommandName == "Bootstrapped")
            {
                SystemManager.RegisterWebService(typeof(ReorderDynamicContentService),
                    Installer.reorderDynamicContentServiceUrl);
            }  
        }

        public static void RegisterDialog(object sender, ExecutingEventArgs e)
        {
            if (e.CommandName == "RegisterDialogs")
            {
                Dialogs.RegisterDialog<ReorderDynamicContent.ReorderDialogCustom>();
            }
        }

        //Example: Installer.InstallExtensionScript("Telerik.Sitefinity.DynamicTypes.Model.Merchants.OutletBackendDefinition", "OutletBackendList",
        //"Telerik.Sitefinity.DynamicTypes.Model.Merchants.Outlet", "Outlets")
        public static void Install(string contentViewControlName, string viewName, string itemType, string itemNameWithArticle)
        {
            Installer.InstallExtensionScript(contentViewControlName, viewName,
                Installer.ScriptReference, Installer.loadMethodName);

            Installer.InstallToolbarCommand(contentViewControlName, viewName);
            Installer.InstallReorderDialog(contentViewControlName, viewName, itemType, itemNameWithArticle);
        }

        public static void InstallExtensionScript(string contentViewControlName,
            string viewName, string scriptLocation, string loadMethodName)
        {
            var configManager = ConfigManager.GetManager();
            using (new ElevatedModeRegion(configManager))
            {
                ContentViewConfig config;
                var masterView = Installer.GetContentViewElement<MasterGridViewElement>(configManager, contentViewControlName, viewName, out config);
                var script = masterView.Scripts.Elements
                    .Where(e => e.ScriptLocation == scriptLocation)
                    .FirstOrDefault();

                if (script == null)
                {
                    var clientScript = new ClientScriptElement(masterView.Scripts);
                    clientScript.ScriptLocation = scriptLocation;
                    clientScript.LoadMethodName = loadMethodName;
                    clientScript.CollectionItemName = "script";
                    masterView.Scripts.Add(clientScript);

                    configManager.SaveSection(config);
                }
            }
            configManager.Dispose();
        }

        protected static T GetContentViewElement<T>(ConfigManager configManager, string contentViewControlName,
    string viewName, out ContentViewConfig config)
            where T : ContentViewDefinitionElement
        {
            config = configManager.GetSection<ContentViewConfig>();
            var contentBackend = config.ContentViewControls[contentViewControlName];
            var view = contentBackend.ViewsConfig.Values.FirstOrDefault(v => v.ViewName == viewName);
            var viewElement = (T)view;

            return viewElement;
        }

        public static void InstallToolbarCommand(string contentViewControlName, string viewName)
        {
            var configManager = ConfigManager.GetManager();
            using (new ElevatedModeRegion(configManager))
            {
                ContentViewConfig config;
                var masterView = Installer.GetContentViewElement<MasterGridViewElement>(configManager, contentViewControlName, viewName, out config);

                var toolbar = masterView.Toolbar.Sections.Where(s => s.Name == "toolbar").FirstOrDefault();
                var command = toolbar.Items.Where(i => i.Name == "reorder").FirstOrDefault();

                if (command == null)
                {
                    var toolbarElement = masterView.ToolbarConfig.WidgetSections.Where(s => s.Name == "toolbar").FirstOrDefault();
                    var commandWidget = new Telerik.Sitefinity.Web.UI.Backend.Elements.Config.CommandWidgetElement(toolbarElement.Items);
                    commandWidget.CommandArgument = "";
                    commandWidget.CommandName = "reorder";
                    commandWidget.Name = "reorder";
                    commandWidget.Text = "Reorder items";
                    commandWidget.WrapperTagKey = HtmlTextWriterTag.Li;
                    commandWidget.ButtonType = Telerik.Sitefinity.Web.UI.Backend.Elements.Enums.CommandButtonType.Standard;
                    commandWidget.WidgetType = typeof(CommandWidget);
                    toolbarElement.Items.Add(commandWidget);
                 
                    configManager.SaveSection(config);
                }
            }
            configManager.Dispose();
        }

        public static void InstallReorderDialog(string contentViewControlName,
            string viewName, string itemType, string itemNameWithArticle)
        {
            var configManager = ConfigManager.GetManager();
            using (new ElevatedModeRegion(configManager))
            {
                ContentViewConfig config;
                var masterView = Installer.GetContentViewElement<MasterGridViewElement>(configManager, contentViewControlName, viewName, out config);

                var dialog = masterView.DialogsConfig.Elements
                    .Where(e => e.Name == "ReorderDialogCustom" && e.OpenOnCommandName == "reorder")
                    .FirstOrDefault();

                if (dialog == null)
                {
                    dialog = new Telerik.Sitefinity.Web.UI.Backend.Elements.Config.DialogElement(masterView.DialogsConfig);
                    //dialog.ID will be generated automatically
                    dialog.Width = new System.Web.UI.WebControls.Unit("100%");
                    dialog.Height = new System.Web.UI.WebControls.Unit("100%");
                    dialog.Behaviors = Telerik.Web.UI.WindowBehaviors.None;
                    dialog.InitialBehaviors = Telerik.Web.UI.WindowBehaviors.Maximize;
                    dialog.Skin = "Default";
                    dialog.OpenOnCommandName = "reorder";
                    dialog.Name = "ReorderDialogCustom";
                    //Example: "?itemType=Telerik.Sitefinity.DynamicTypes.Model.Merchants.Outlet&itemNameWithArticle=Outlets"
                    string reorderDialogParameters = string.Format("?itemType={0}&itemNameWithArticle={1}", itemType, itemNameWithArticle);
                    dialog.Parameters = reorderDialogParameters;

                    masterView.DialogsConfig.Add(dialog);

                    configManager.SaveSection(config);
                }
            }
            configManager.Dispose();
        }

        internal const string ScriptReference = "~/ReorderDynamicContent/ExtScript.js";
        private static readonly string loadMethodName = "OnModuleMasterViewLoadedCustom";
        internal const string reorderDynamicContentServiceUrl = "Sitefinity/Services/ReorderDynamicContentService.svc";
    }
}