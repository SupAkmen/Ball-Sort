using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class GradientButton : MonoBehaviour
{
    public Color topColor = Color.cyan;    // Màu trên (có thể tùy chỉnh trong Inspector)
    public Color bottomColor = Color.blue; // Màu dưới (có thể tùy chỉnh trong Inspector)
    public float gradientSpeed = 1f;       // Tốc độ đổi màu

    private Image buttonImage;
    private Material gradientMaterial;

    void Start()
    {
        // Lấy thành phần Image của nút
        buttonImage = GetComponent<Image>();

        // Tạo Material mới cho gradient
        gradientMaterial = new Material(Shader.Find("Unlit/NewUnlitShader"));
        buttonImage.material = gradientMaterial;
    }

    void Update()
    {
        // Cập nhật màu gradient động theo thời gian
        float lerpValue = Mathf.PingPong(Time.time * gradientSpeed, 1f);
        gradientMaterial.SetColor("_TopColor", Color.Lerp(topColor, bottomColor, lerpValue));
        gradientMaterial.SetColor("_BottomColor", Color.Lerp(bottomColor, topColor, lerpValue));
    }
}
