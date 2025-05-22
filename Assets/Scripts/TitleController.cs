using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleController : MonoBehaviour
{
    [Header("タイトル系オブジェクト")]
    public GameObject title_objects;
    [Header("UI系オブジェクト")]
    public GameObject ui_objects;

    public StageController stageController;

    // Start is called before the first frame update
    void Start()
    {
        title_objects.SetActive(true);
        ui_objects.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            stageController.timeline.Play();
            title_objects.SetActive(false);
            ui_objects.SetActive(true);
        }

    }
}
