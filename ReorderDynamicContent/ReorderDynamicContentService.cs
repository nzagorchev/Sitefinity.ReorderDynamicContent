using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using Telerik.Sitefinity.DynamicModules;
using Telerik.Sitefinity.DynamicModules.Model;
using Telerik.Sitefinity.GenericContent.Model;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.Utilities.TypeConverters;
using Telerik.Sitefinity.Web.Services;

namespace SitefinityWebApp.ReorderDynamicContent
{
    [System.ServiceModel.ServiceBehavior]
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class ReorderDynamicContentService
    {
        [WebInvoke(Method = "PUT", ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "ReorderDynamicContent/?itemType={itemType}&provider={provider}")]
        public void ReorderDynamicContent(Dictionary<string, float> contentIdnewOrdinal, string provider, string itemType)
        {
            ServiceUtility.RequestBackendUserAuthentication();
            var manager = DynamicModuleManager.GetManager(provider);
            var type = TypeResolutionService.ResolveType(itemType);

            foreach (KeyValuePair<string, float> pair in contentIdnewOrdinal)
            {
                var id = new Guid(pair.Key);
                var item = manager.GetDataItem(type, id);

                // Do not publish the item - it could be locked or a draft
                item.SetValue("Ordinal", pair.Value);
                DynamicContent other = null;
                if (item.Status == ContentLifecycleStatus.Master)
                {
                    other = manager.Lifecycle.GetLive(item) as DynamicContent;
                }
                else
                {
                    other = manager.Lifecycle.GetMaster(item) as DynamicContent;
                }

                if (other != null)
                {
                    other.SetValue("Ordinal", pair.Value);
                }

                var temp = manager.Lifecycle.GetTemp(item) as DynamicContent;
                if (temp != null)
                {
                    temp.SetValue("Ordinal", pair.Value);
                } 
            }

            manager.SaveChanges();
        }
    }
}