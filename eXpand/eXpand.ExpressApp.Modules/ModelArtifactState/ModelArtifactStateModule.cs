using DevExpress.ExpressApp;

namespace eXpand.ExpressApp.ModelArtifactState
{
    public sealed partial class ModelArtifactStateModule : ModuleBase
    {
        public const string ModelArtifactStateAttributeName = "ModelArtifactState";
        public ModelArtifactStateModule()
        {
            InitializeComponent();
        }
        //        private void application_LoggedOn(object sender, LogonEventArgs e)
        //        {
        //            CollectRules(((XafApplication)sender));
        //        }
        //private void application_SetupComplete(object sender, EventArgs e)
        //{
        //    CollectRules(((XafApplication)sender));
        //}
        //        public static ModelArtifactStateNodeWrapper CreateModelWrapper(Dictionary dictionary)
        //        {
        //            if (dictionary != null){
        //                return
        //                    new ModelArtifactStateNodeWrapper(
        //                        dictionary.RootNode.GetChildNode(ModelArtifactStateNodeWrapper.NodeNameAttribute));
        //            }
        //            return null;
        //        }

        public static void CollectRules(XafApplication xafApplication)
        {
            //            lock (ArtifactStateRuleManager.Instance){
            //                SecuritySystem.ReloadPermissions();
            //                ModelArtifactStateNodeWrapper wrapper = CreateModelWrapper(xafApplication.Model);
            //                foreach (ITypeInfo typeInfo in XafTypesInfo.Instance.PersistentTypes){
            //                    ArtifactStateRuleManager.Instance[typeInfo] = null;
            //                    List<ArtifactStateRule> enumerable = CollectRulesFromModelCore(wrapper, typeInfo, xafApplication).ToList();
            //                    List<ArtifactStateRule> permissions = ArtifactStateRuleManager.FillRulesFromPermissions(xafApplication, typeInfo).ToList();
            //                    enumerable.AddRange(permissions);
            //                    ArtifactStateRuleManager.Instance[typeInfo] = new List<ArtifactStateRule>(enumerable);
            //                }
            //            }
        }
        //        private static IEnumerable<ArtifactStateRule> CollectRulesFromModelCore(ModelArtifactStateNodeWrapper wrapper, ITypeInfo typeInfo, XafApplication application)
        //        {
        //            foreach (ArtifactStateRuleNodeWrapper rule in wrapper.FindRules(typeInfo))
        //            {
        //                var artifactStateRule = (ArtifactStateRule)rule;
        //                if (artifactStateRule is ControllerStateRule)
        //                {
        //                    ArtifactStateRuleNodeWrapper nodeWrapper = rule;
        //                    ((ControllerStateRule)artifactStateRule).ControllerType =
        //                        application.Modules[0].ModuleManager.ControllersManager.CollectControllers(
        //                            info => info.FullName == ((ControllerStateRuleNodeWrapper)nodeWrapper).ControllerType).
        //                            Single().GetType();
        //                }
        //                yield return artifactStateRule;
        //            }
        //        }

        //        public override void Setup(XafApplication application)
        //        {
        //            base.Setup(application);
        //            application.SetupComplete += application_SetupComplete;
        //            application.LoggedOn += application_LoggedOn;
        //        }
        //        public override void ValidateModel(Dictionary model)
        //        {
        //            base.ValidateModel(model);
        //            var wrapper = new ApplicationNodeWrapper(model);
        //            var nodeWrappers = new List<NodeWrapper>();
        //            nodeWrappers.AddRange(wrapper.BOModel.Classes.Cast<NodeWrapper>());
        //            nodeWrappers.AddRange(wrapper.Views.Items.Cast<NodeWrapper>());
        //            foreach (NodeWrapper nodeWrapper in nodeWrappers)
        //            {
        //                validate(getWrappers<ActionStateRulesNodeWrapper>(nodeWrapper, ActionStateRulesNodeWrapper.NodeNameAttribute));
        //                validate(getWrappers<ControllerStateRulesNodeWrapper>(nodeWrapper, ControllerStateRulesNodeWrapper.NodeNameAttribute));
        //            }
        //
        //
        //        }

        //        private void validate(RulesNodeWrapper<ArtifactStateRuleNodeWrapper> conditionalArtifactStateNodeWrapper)
        //        {
        //            if (conditionalArtifactStateNodeWrapper is ActionStateRulesNodeWrapper)
        //                foreach (ActionStateRuleNodeWrapper actionStateRuleNodeWrapper in conditionalArtifactStateNodeWrapper.Rules)
        //                {
        //                    if (string.IsNullOrEmpty(actionStateRuleNodeWrapper.ActionId) && string.IsNullOrEmpty(actionStateRuleNodeWrapper.Module))
        //                        throw new DictionaryValidationException("At least one of ActionId,Module attributes should have values ");
        //                }
        //            if (conditionalArtifactStateNodeWrapper is ControllerStateRulesNodeWrapper)
        //                foreach (ControllerStateRuleNodeWrapper actionStateRuleNodeWrapper in conditionalArtifactStateNodeWrapper.Rules)
        //                {
        //                    if (string.IsNullOrEmpty(actionStateRuleNodeWrapper.ControllerType) && string.IsNullOrEmpty(actionStateRuleNodeWrapper.Module))
        //                        throw new DictionaryValidationException("At least one of ControllerType,Module attributes should have values ");
        //                }
        //        }

        //        private TConditionalArtifactStateNodeWrapper getWrappers<TConditionalArtifactStateNodeWrapper>(NodeWrapper nodeWrapper, string nodeName)
        //            where TConditionalArtifactStateNodeWrapper : ConditionalArtifactStateNodeWrapper
        //        {
        //            DictionaryNode node = nodeWrapper.Node.FindChildNode(nodeName);
        //            if (node != null)
        //                return
        //                    (TConditionalArtifactStateNodeWrapper)
        //                    Activator.CreateInstance(typeof(TConditionalArtifactStateNodeWrapper), new[] { node });
        //            return null;
        //        }


    }
}
