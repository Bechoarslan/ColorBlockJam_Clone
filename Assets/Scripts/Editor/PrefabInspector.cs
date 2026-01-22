using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class PrefabInspector
    {
        private LevelEditor Editor;

        public PrefabInspector(LevelEditor editor)
        {
            Editor = editor;
        }

        public void DrawRotatedPreview(GameObject prefab, float rotationY)
        {
            if (prefab == null) return;

            // 1. Preview Utility yoksa oluştur
            if (Editor._previewUtility == null)
            {
                Editor._previewUtility = new PreviewRenderUtility();

                // Arka plan rengi
                Editor._previewUtility.camera.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 1f);
                Editor._previewUtility.camera.clearFlags = CameraClearFlags.SolidColor;

                // --- BU KISMI DEĞİŞTİRİYORUZ ---

                // 1. Modu Orthographic yap (Perspektifi kapatır, teknik çizim gibi düz gösterir)
                Editor._previewUtility.camera.orthographic = true;

                // 2. Zoom ayarı (Bu sayıyı artırırsan uzaklaşır, azaltırsan yakınlaşır)
                Editor._previewUtility.camera.orthographicSize = 2.0f;

                // 3. Pozisyon: Tam merkezde (0,0) ama YUKARIDA (Y=10)
                Editor._previewUtility.camera.transform.position = new Vector3(0, 10, 0);

                // 4. Rotasyon: X ekseninde 90 derece eğ (Tam aşağı bak)
                // Y eksenini 0 yapıyoruz ki "Yukarı" yönü Dünya'nın "İleri" (Z) yönü olsun.
                Editor._previewUtility.camera.transform.rotation = Quaternion.Euler(90, 90, 0);

                // 5. Kesme düzlemleri (Kameranın ne kadar yakını/uzağı göreceği)
                Editor._previewUtility.camera.nearClipPlane = 0.1f;
                Editor._previewUtility.camera.farClipPlane = 20f;
            }
            // ...

            // 2. Çizim alanını al (Inspector'da 200x200 kare yer ayır)
            Rect rect = EditorGUILayout.GetControlRect(false, 200);

            // 3. Render İşlemini Başlat
            Editor._previewUtility.BeginPreview(rect, GUIStyle.none);

            // 4. Bloğun içindeki tüm parçaları (Mesh) bul ve çiz
            // Senin kodun "Line Block" olduğu için child objeleri taramamız lazım.
            MeshFilter[] filters = prefab.GetComponentsInChildren<MeshFilter>();

            // Dönme işlemini burada yapıyoruz!
            Quaternion rot = Quaternion.Euler(0, rotationY, 0);

            foreach (var filter in filters)
            {
                Mesh mesh = filter.sharedMesh;

                // Materyali al (Yoksa standart pembe materyal olmasın diye default ata)
                Material mat = filter.GetComponent<Renderer>()?.sharedMaterial;
                if (mat == null) mat = new Material(Shader.Find("Standard"));

                // Objenin yerel pozisyonunu, bizim döndürdüğümüz ana rotasyonla çarp
                // Bu matematik: Child'ın yerini, ana objenin dönüşüne göre ayarlar.
                Vector3 finalPos = rot * filter.transform.localPosition;

                // Çiz (Mesh, Pozisyon, Rotasyon, Materyal, Layer)
                Editor._previewUtility.DrawMesh(mesh, Matrix4x4.TRS(finalPos, rot, Vector3.one), mat, 0);
            }

            // 5. Kamerayı render et ve sonucu al
            Editor._previewUtility.camera.Render();
            Texture resultTexture = Editor._previewUtility.EndPreview();

            // 6. Sonucu ekrana bas
            GUI.DrawTexture(rect, resultTexture, ScaleMode.ScaleToFit);
        }
    }
}