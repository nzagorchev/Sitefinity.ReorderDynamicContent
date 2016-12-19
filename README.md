# Sitefinity.ReorderDynamicContent
Implementation for reordering of dynamic content items. It works for both one level and hierarchical dynamic modules. The reordering happens using drag &amp; drop - the same as the out of the box ordering for media content.

# Prerequisites:
Set the dynamic module an "Ordinal" field and populate all items ordinal with an unique value. You can also assign all new items Ordinal equal to the Max one +1 of the current items, so they come last in the ordering.

# Installation:
Set the installer to add the dialog, an extension script to handle the reorder command and a widget on the toolbar. Set the items backend view name (can be seen from Advanced settings -> ContentView -> controls), the listview definition name and the module itemType. The itemNameWithArticle indicates the text that will be shown on the reordering screen.

```cs
       protected void Application_Start(object sender, EventArgs e)
       {
           Bootstrapper.Initialized += new EventHandler<ExecutedEventArgs>(Bootstrapper_Initialized);
		   // Install the reordering handlers
           Installer.InstallInfrastructure();
       }
 
       protected void Bootstrapper_Initialized(object sender, ExecutedEventArgs e)
       {
           if (e.CommandName == "Bootstrapped")
           {
               // Register the reordering for specific module           ReorderDynamicContent.Installer.Install("Telerik.Sitefinity.DynamicTypes.Model.Merchants.OutletBackendDefinition", "OutletBackendList",
       "Telerik.Sitefinity.DynamicTypes.Model.Merchants.Outlet","Outlets");
           }          
       }
```

![alt tag](https://github.com/nzagorchev/Sitefinity.ReorderDynamicContent/blob/master/reorder_dynamic_gif.gif)