using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MapCreate : MonoBehaviour
{
    // CSV ���� �Ľ� ���� ���� ���� data_Stage[��][��]
    List<Dictionary<string, object>> data_Stage = new List<Dictionary<string, object>>();
    [SerializeField]
    List<GameObject> renderObj = new List<GameObject>(); // ���� ���� render���� ������Ʈ��

    private float mapX;
    private float mapY;
    private Vector2 renderPos; // � ��ǥ�� ���� render�ؾ��ϴ°�
    private GameObject mapBox; // ��Ƶ� �ڽ�

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            GameManager.currentStage++;
            Initialize();
        }
    }

    public void Initialize()
    {
        data_Stage = CSVReader.Read("SN_" + GameManager.currentScenario.ToString() + "_ST_" + GameManager.currentStage.ToString());
        MapRender();
    }

    private void MapRender()
    {
        mapBox = GameObject.Find("Maps");
        // �� �ʱ�ȭ
        foreach(Transform child in mapBox.transform)
        {
            Destroy(child.gameObject);
        }
        // X, Y �ִ� ���� ����
        mapX = data_Stage[0].Count - 1;
        mapY = data_Stage.Count - 1;

        renderPos = new Vector2(-GameManager.gridSize * (mapX / 2), GameManager.gridSize * (mapY / 2));

        for(int countX = 0; countX < data_Stage.Count; countX++) // ���� �ִ� ��� ���� �� ���� �� ��� �� �� 1ĭ �̵�
        {
            foreach(KeyValuePair<string, object> child in data_Stage[countX]) // ���� �ش��ϴ� �� ���
            {
                // �ƹ��͵� �ȵ� �ִ� ��� �н�
                if(child.Value == null)
                {
                    renderPos.x += GameManager.gridSize;
                    continue;
                }
                // �ϴ� _ ��ȣ �������� ���ڿ� ���ø�
                string[] splitText = child.Value.ToString().Split('_');

                // ���� ������Ʈ�� �����Ǿ�� �Ҷ�
                if(int.Parse(splitText[0].ToString()) == 3)
                {
                    GameObject tmpObj = Instantiate(renderObj[int.Parse(splitText[0].ToString())], renderPos, Quaternion.identity, mapBox.transform);
                    tmpObj.GetComponent<ObjectData>().num = int.Parse(splitText[1].ToString());
                    tmpObj.transform.GetChild(0).GetComponent<TMP_Text>().text = splitText[1]; 
                }
                else
                {
                    Instantiate(renderObj[int.Parse(splitText[0].ToString())], renderPos, Quaternion.identity, mapBox.transform);
                }
                Instantiate(renderObj[0], renderPos, Quaternion.identity, mapBox.transform);

                renderPos.x += GameManager.gridSize;
            }
            renderPos.x = -GameManager.gridSize * (mapX / 2);
            renderPos.y -= GameManager.gridSize;
        }
    }
}