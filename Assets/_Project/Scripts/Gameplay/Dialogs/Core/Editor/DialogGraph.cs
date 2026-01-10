using System;
using Unity.GraphToolkit.Editor;
using UnityEditor;

namespace _Project.Gameplay.Dialogs.Core.Editor
{
    [Serializable, Graph(AssetExtension)]
    public class DialogGraph : Graph
    {
        public const string AssetExtension = "dialoggraph";

        [MenuItem("Assets/Create/Dialogs/Dialog Graph", false)]
        private static void CreateAssetFile()
        {
            GraphDatabase.PromptInProjectBrowserToCreateNewAsset<DialogGraph>();
        }
    }
}
