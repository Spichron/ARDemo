using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainController : MonoBehaviour
{
    // Projede hedef (image target) olan Drone, merkez (world origin) noktası, astronot da asıl hedef olarak kullanılmıştır. 
    // Çünkü merkez noktalar sabittir ve döndürüldüklerinde veya hareket ettirildiklerinde kendileri değil diğer çevre birimleri değiştirilmiş olarak anlaşılmaktadır.
    // Bundan dolayı hedef döndüğünde yönü anlaşılabilsin ve kameranın dönmesiyle bu değeri koruyabilsindiye merkezi üstlenecek ikinci bir hedef seçilmiştir.
    
    // Hedef ve Merkezin takip yazıları Inspector'da "Default Trackable Event Handler" tarafından düzenlenmektedir.

    // Hareket sırasında ekranın dönmemesi için proje oryantasyonu Landscape Right olarak kilitlenmiştir.

    private GameObject selectedObject1; // Hedef Pusulasının Konumunu referans eden obje
    private GameObject selectedObject2; // Kamera Pusulasının Konumunu referans eden obje

    private GameObject arrow1; // Hedef yönünü gösteren ok
    private GameObject arrow2; // Kamera yönünü gösteren ok

    private GameObject compass1; // Hedef Pusulası Görseli
    private GameObject compass2; // Kamera Pusulası Görseli

    private GameObject text1; // Hedef Yazısı
    private GameObject text2; // Kamera Yazısı

    private GameObject main1; // Hedef verilerinin Parent Objesi
    private GameObject main2; // Kamera verilerinin Parent Objesi

    private GameObject mainText; // Hedefin görüşte olup olmadığının Yazısı
    private GameObject originText; // Hedefin görüşte olup olmadığının Yazısı

    private GameObject screenCanvas; // Canvas Objesi

    private GameObject realImage; // Hedef

    private Vector3 mousePosition; 

    private int clickCount = 0; // Ekrana kaç kere tıklanıldığını Tutar

    private void Start()
    {
        // Obje Tanımlamaları
        selectedObject1 = GameObject.Find("SelectedObject1");
        selectedObject2 = GameObject.Find("SelectedObject2");
        compass1 = GameObject.Find("Compass1");
        compass2 = GameObject.Find("Compass2");
        text1 = GameObject.Find("Text1");
        text2 = GameObject.Find("Text2");
        main1 = GameObject.Find("Main1");
        main2 = GameObject.Find("Main2");
        arrow1 = GameObject.Find("Arrow1");
        arrow2 = GameObject.Find("Arrow2");
        mainText = GameObject.Find("DetectionText");
        originText = GameObject.Find("OriginText");
        screenCanvas = GameObject.Find("Canvas");
        realImage = GameObject.Find("ImageTarget");
        screenCanvas.GetComponent<CanvasScaler>().referenceResolution = new Vector2 (Screen.width, Screen.height); //UI elemanlarının her cihaz ekranına oturabilmesi için Canvas cihaz ekranı boyutlarına göre ayarlanıyor.
    }

    private void Update()
    {
        HitTarget(); // Hedef üzerine tıklanan noktaların seçimi
        PoseAndRotate(); // Pusulaların konumve imleçlerinin yönlerinin ayarlanması
        HideAndReveal(); // Hedef takipte değilken görsellerin ekrandan kaybolması
    }

    private void HitTarget()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mousePosition = Input.mousePosition;
            mousePosition.z = 1.0f; // Bu pozisyon ayarlaması sayesinde kamera görüşünden bir kesit alınmış olur, böylece mouse verisi başarılı şekilde World konumuna iletilebilir.
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity)) //Rigidbody'e sahip tek obje hedefin üstünde hedefle aynı boyutta olan görünmez bir obje
            {
                if (clickCount == 0) //İlk tıklamada hedef pusulasının konumu belirleniyor. Görünmez olan Pusula görünür hale getiriliyor.
                {
                    selectedObject1.transform.position = hit.point;
                    if (compass1.GetComponent<Image>().color.a == 0)
                    {
                        compass1.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                        text1.GetComponent<Text>().color = new Color(0, 1, 0, 1);
                        arrow1.GetComponent<Image>().color = new Color(0, 1, 0, 1);
                    }
                    clickCount++;
                }
                else if (clickCount == 1) //İkinci tıklamada kamera pusulasının konumu belirleniyor. Görünmez olan Pusula görünür hale getiriliyor.
                {
                    selectedObject2.transform.position = hit.point;
                    if (compass2.GetComponent<Image>().color.a == 0)
                    {
                        compass2.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                        text2.GetComponent<Text>().color = new Color(0, 0, 1, 1);
                        arrow2.GetComponent<Image>().color = new Color(0, 0, 1, 1);
                    }
                    clickCount++;
                }
            }
        }
    }

    private void PoseAndRotate()
    {
        main1.transform.position = Camera.main.WorldToScreenPoint(selectedObject1.transform.position); //Pusula, refeans objelerinin konumlarının ekrana yansıyan noktasına oturur. 
        main2.transform.position = Camera.main.WorldToScreenPoint(selectedObject2.transform.position); 

        arrow1.transform.localEulerAngles = new Vector3(0f, 180f, realImage.transform.localEulerAngles.y - Camera.main.transform.rotation.eulerAngles.y); //Pusula oku hedefin baktığı yönü gösterecek şekilde ayarlanmıştır. Ancak Kamera dönüşü ekranı da çevirdiği için bu dönüş yön değerinden çıkarılmıştır.
        arrow2.transform.localEulerAngles = new Vector3(0f, 0f, Camera.main.transform.localEulerAngles.z); //Pusula kameranın bakış yönünü gösterir.
    }

    private void HideAndReveal()
    {
        if (clickCount >= 1) //Eğer merkez ile hedef takipteyse ve pusula konumları seçilmişse pusulalar görünür.
        {
            if (mainText.GetComponent<Text>().text == "İşaretçi takip ediliyor.." && originText.GetComponent<Text>().text == "Merkez takip ediliyor..") 
            {
                compass1.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                text1.GetComponent<Text>().color = new Color(0, 1, 0, 1);
                arrow1.GetComponent<Image>().color = new Color(0, 1, 0, 1);
            }
            else
            {
                compass1.GetComponent<Image>().color = new Color(1, 1, 1, 0);
                text1.GetComponent<Text>().color = new Color(0, 1, 0, 0);
                arrow1.GetComponent<Image>().color = new Color(0, 1, 0, 0);
            }
        }
        if (clickCount == 2)
        {
            if (mainText.GetComponent<Text>().text == "İşaretçi takip ediliyor.." && originText.GetComponent<Text>().text == "Merkez takip ediliyor..")
            {
                compass2.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                text2.GetComponent<Text>().color = new Color(0, 0, 1, 1);
                arrow2.GetComponent<Image>().color = new Color(0, 0, 1, 1);
            }
            else
            {
                compass2.GetComponent<Image>().color = new Color(1, 1, 1, 0);
                text2.GetComponent<Text>().color = new Color(0, 0, 1, 0);
                arrow2.GetComponent<Image>().color = new Color(0, 0, 1, 0);
            }
        }
    }
}
