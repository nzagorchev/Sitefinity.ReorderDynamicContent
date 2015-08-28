using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using Telerik.Sitefinity.DynamicModules.Model;
using Telerik.Sitefinity.Localization;
using Telerik.Sitefinity.Modules.Libraries;
using Telerik.Sitefinity.Modules.Libraries.Web.UI;
using Telerik.Sitefinity.Web.UI;

namespace SitefinityWebApp.ReorderDynamicContent
{
    public class ReorderDialogCustom : ReorderDialog
    {
        protected override void InitializeControls(Telerik.Sitefinity.Web.UI.GenericContainer container)
        {
            // ItemType should be resolved from the dialog parameters for each module
            if (this.ItemType.BaseType != null && this.ItemType.BaseType.FullName == typeof(DynamicContent).FullName)
            {
                var binderContainer = this.ImageBinder.Containers.Where<BinderContainer>(c => c.ID == "BinderContainer1").FirstOrDefault();
                if (binderContainer != null)
                {
                    binderContainer.Markup =
                        @"<span sys:title=""{{Title.PersistedValue}}"" style=""border:1px solid #000"" >{{(Title.PersistedValue.length > 22) ? Title.PersistedValue.substring(0,22) + '...' : Title.PersistedValue}}</span>";
                }

                this.ImageBinder.HandleItemReordering = true;
                this.ImageBinder.AutoUpdateOrdinals = false;
                this.ImageBinder.ServiceUrl = this.GetServiceUrl();
                this.ImageBinder.ServiceChildItemsBaseUrl = this.GetChildServiceUrl();
                this.BackButton.Text = string.Format(Res.Get<Labels>().BackToAllItemsParameter, this.ItemsName);
                DragToChangeOrderLiteral.Text = string.Format(Res.Get<LibrariesResources>().DragToChangeOrder, this.ItemNameWithArticle);
            }
            else
            {
                base.InitializeControls(container);
            }
        }

        protected virtual string GetChildServiceUrl()
        {
            return ReorderDialogCustom.childServiceUrl;
        }

        protected virtual string GetServiceUrl()
        {
            return ReorderDialogCustom.serviceUrl;
        }

        public override IEnumerable<ScriptReference> GetScriptReferences()
        {
            var scripts = base.GetScriptReferences().ToList();
            scripts.Add(new ScriptReference(ReorderDialogCustom.dialogScript));
            return scripts;
        }

        public override IEnumerable<System.Web.UI.ScriptDescriptor> GetScriptDescriptors()
        {
            var @base = base.GetScriptDescriptors();

            var descriptor = (ScriptControlDescriptor)@base.Last();
            descriptor.Type = typeof(ReorderDialogCustom).FullName;
            descriptor.AddProperty("_webServiceUrl", this.GetServiceUrl());
            descriptor.AddProperty("_webSaveServiceUrl", ReorderDialogCustom.reorderDynamicContentServiceUrl);

            return @base;
        }

        internal const string dialogScript = "~/ReorderDynamicContent/ReorderDialogCustom.js";
        protected static readonly string reorderDynamicContentServiceUrl = "/Sitefinity/Services/ReorderDynamicContentService.svc";
        protected static readonly string serviceUrl = "/Sitefinity/Services/DynamicModules/Data.svc/?managerType=Telerik.Sitefinity.DynamicModules.DynamicModuleManager";
        protected static readonly string childServiceUrl = "/Sitefinity/Services/DynamicModules/Data.svc/parent/?managerType=Telerik.Sitefinity.DynamicModules.DynamicModuleManager";
    }
}