using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SFB;
using UnityEngine;

public class RunnerStart : MonoBehaviour
{
    public GameObject ComponentsContainer;
    IDataService DataService = new DataService();


    // Start is called before the first frame update
    void Start()
    {
        LoadSavedScene();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LoadSavedScene()
    {
        StandaloneFileBrowser.OpenFilePanelAsync("Open File", "", "json", false, (string[] paths) => {
            
            SaveMetadata loadedSaveData = this.DataService.LoadData<SaveMetadata>(paths[0]);

            GameObject savedGameObjectContainer = new GameObject( loadedSaveData.Name );
            savedGameObjectContainer.transform.position = Vector3.zero;
            savedGameObjectContainer.tag = "Component";

            foreach (Component component in loadedSaveData.Components )
            {
                GameObject newGameObject = CreateGameObjectFromUnivComponent(component);
                
                if ( ComponentsContainer.transform.childCount == 0 ){
                    newGameObject.transform.SetParent(savedGameObjectContainer.transform);
                }
            }

            savedGameObjectContainer.transform.SetParent(ComponentsContainer.transform);

        });
    }

    GameObject CreateGameObjectFromUnivComponent ( Component component ) {

        // Create Game Object
        var newGameObject = new GameObject();
        newGameObject.name = component.Name;
        
        newGameObject.transform.position = new Vector3(component.RelativePosition.X, component.RelativePosition.Y, component.RelativePosition.Z);
        newGameObject.transform.Rotate(new Vector3(component.Rotation.X,component.Rotation.Y,component.Rotation.Z)); 
        newGameObject.transform.localScale = new Vector3( component.Scale.X, component.Scale.Y, component.Scale.Z );

        var gltf = newGameObject.AddComponent<GLTFast.GltfAsset>();
        gltf.Url = component.LocalModelFilePath;
        newGameObject.tag = "Component";

        int currentChildIndex = 0;
        while( currentChildIndex != component.Components.Count() ) {
            
            Component currentChildComponent = component.Components[currentChildIndex];
            GameObject childComponentGameObject = CreateGameObjectFromUnivComponent(currentChildComponent);
            childComponentGameObject.transform.SetParent(newGameObject.transform);
            currentChildIndex++;
        }
        
        return newGameObject;

    }

    private string getPathFromFileName(string name) => Application.dataPath + (Application.dataPath.Contains("/") ? $"/Models/{name}" : $"\\Models\\{name}");
}
