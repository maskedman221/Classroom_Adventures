using UnityEngine;
using UnityEngine.UI;

public class ImageHolder : MonoBehaviour
{
  [SerializeField] private Image image;


private void Update(){
  if(ImageGameManager.Instance.GetisGamePlaying()){
    image.gameObject.SetActive(false);
  }
}
}
