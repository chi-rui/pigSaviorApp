using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;


public class registered : MonoBehaviour{

    public Text new_account, new_passwd;
    public GameObject warningPanel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void newRegister(){
        string account = new_account.text;
        string passwd = new_passwd.text;
        StartCoroutine(newRegister(account, passwd));
    }

    IEnumerator newRegister(string account, string passwd) {

        print(account + " / " + passwd);
        string URL = "http://163.21.245.192/PigSaviorAPP/userRegister.php";
        
        WWWForm form = new WWWForm();
        form.AddField("new_account", account);
        form.AddField("new_passwd", passwd);

        // print(form);

        using(UnityWebRequest www = UnityWebRequest.Post(URL, form)){

            yield return www.SendWebRequest();

            if(www.isNetworkError || www.isHttpError) {
                print(www.error);
            }else{
                print(www.downloadHandler.text);
                if(www.downloadHandler.text == "成功建立帳號！"){
                    warningPanel.SetActive(true);
                    warningPanel.transform.GetChild(1).GetComponent<Text>().text = "成功建立帳號！趕快登入遊玩吧！";
                    new_account.text = new_passwd.text = "";
                    GameObject.Find("Panel_register").SetActive(false);
                }else{
                    warningPanel.SetActive(true);
                    warningPanel.transform.GetChild(1).GetComponent<Text>().text = www.downloadHandler.text;
                }
            }
        
        }
    }

    // public void cleanInput( Text t ){
    //     t.text = "";
    // }
}
