/*
 * Created by Wes McDermott - 2011 - the3dninja.com/blog
 */

using UnityEditor;
using UnityEngine;


public class DistanceToolMenu : MonoBehaviour
{
    [MenuItem("GameObject/The 3D Ninja/Create Distance Tool")]
    private static void CreateDistanceTool()
    {
        if (Selection.activeGameObject != null)
        {
            //Did the user select a DistanceTool?
            if (Selection.activeGameObject.name == "DistanceTool")
            {
                addNewDistanceTool(Selection.activeGameObject);
            }
            else
            {
                if (GameObject.Find("DistanceTool") != null)
                {
                    EditorUtility.DisplayDialog("Distance Tool Warning", "Oops, You need to select a Distance Tool to add an additional copy of the tool.", "OK");
                }
                else
                {
                    createNewDistanceTool();
                }
            }
        }
        else
        {
            if (GameObject.Find("DistanceTool") != null)
            {
                addNewDistanceTool(GameObject.Find("DistanceTool"));
            }
            else
            {
                createNewDistanceTool();
            }
        }
    }


    private static void createNewDistanceTool()
    {
        var go = new GameObject("DistanceTool");
        go.transform.position = Vector3.zero;
        go.AddComponent(typeof(DistanceTool));
    }


    private static void addNewDistanceTool(GameObject go)
    {
        go.AddComponent(typeof(DistanceTool));
    }
}