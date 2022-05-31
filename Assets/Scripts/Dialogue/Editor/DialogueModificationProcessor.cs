using System.IO;
using UnityEditor;

namespace RPG.Dialogue.Editor
{
    // dido: this fixes an Dialogue file rename.
    // this was a bug only in old versions of unity.
    // we actually don't need it now and can be removed.
    //public class DialogueModificationProcessor : UnityEditor.AssetModificationProcessor
    //{
    //    private static AssetMoveResult OnWillMoveAsset(string sourcePath, string destinationPath)
    //    {
    //        var dialogue = AssetDatabase.LoadMainAssetAtPath(sourcePath) as Dialogue;

    //        if (dialogue == null)
    //        {
    //            return AssetMoveResult.DidNotMove;
    //        }

    //        if (Path.GetDirectoryName(sourcePath) != Path.GetDirectoryName(destinationPath))
    //        {
    //            return AssetMoveResult.DidNotMove;
    //        }

    //        dialogue.name = Path.GetFileNameWithoutExtension(destinationPath);

    //        return AssetMoveResult.DidNotMove;
    //    }
    //}

}