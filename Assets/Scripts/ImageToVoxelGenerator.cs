using UnityEngine;

public class ImageToVoxelGenerator : MonoBehaviour {
    [SerializeField] private Texture2D image;
    [SerializeField] private QuadtreeComponent quadtree;
    [SerializeField] private float threshold;

    private void Start() {
        Generate();
    }

    private void Generate() {
        var cells = (int)Mathf.Pow(2, quadtree.depth);
        for(int x = 0; x < cells; x++) {
            for(int y = 0; y < cells; y++) {
                Vector2 position = quadtree.transform.position;
                position.x += ((x - cells / 2) / (float)cells) * quadtree.size;
                position.y += ((y - cells / 2) / (float)cells) * quadtree.size;

                var pixel = image.GetPixelBilinear(x / (float)cells, y / (float)cells);
                if(pixel.a > threshold) {
                    quadtree.Quadtree.Insert(position, true);
                }
            }
        }
    }
}
