namespace Dreamteck.Splines
{
    using UnityEngine;
    using TMPro;

    [ExecuteInEditMode]
    public class TextMeshProController : SplineUser
    {
        public enum BendMode
        {
            Character, Distort
        }

        public enum StretchMode
        {
            Original, Stretched
        }

        [SerializeField, HideInInspector] private TMP_Text _tmpro;
        [SerializeField, HideInInspector] private BendMode _bendMode;
        [SerializeField, HideInInspector] private StretchMode _stretchMode;
        [SerializeField, HideInInspector] private Vector2 _offset;
        [SerializeField, HideInInspector, Range(-180, 180)] private float _rotation;

        public TMP_Text target
        {
            get => _tmpro;
            set
            {
                if(value != _tmpro)
                {
                    _tmpro = value;
                    Rebuild();
                }
            }
        }

        public BendMode bendMode
        {
            get => _bendMode;
            set
            {
                if(_bendMode != value)
                {
                    _bendMode = value;
                    Rebuild();
                }
            }
        }

        public StretchMode stretchMode
        {
            get => _stretchMode;
            set
            {
                if (_stretchMode != value)
                {
                    _stretchMode = value;
                    Rebuild();
                }
            }
        }

        public Vector2 offset
        {
            get => _offset;
            set
            {
                if (_offset != value)
                {
                    _offset = value;
                    Rebuild();
                }
            }
        }

        public float rotation
        {
            get => _rotation;
            set
            {
                if(_rotation != value)
                {
                    _rotation = value;
                    Rebuild();
                }
            }
        }

        private string _lastText = "";

        private void Start()
        {
            Rebuild();
        }

        protected override void Run()
        {
            base.Run();
            if(_tmpro != null)
            {
                if (_tmpro.text != _lastText)
                {
                    _lastText = _tmpro.text;
                    Rebuild();
                }
            }
        }

        protected override void Build()
        {
            base.Build();
            if(_tmpro == null)
            {
                return;
            }
            _tmpro.ForceMeshUpdate();
            Vector3[] newVerts = _tmpro.mesh.vertices;
            Vector3[] newNormals = _tmpro.mesh.normals;
            Color[] newColors = _tmpro.mesh.colors;
            var info = _tmpro.textInfo;
            Vector2 pos = new Vector2(_tmpro.rectTransform.rect.x, _tmpro.rectTransform.rect.y);
            Vector2 size = new Vector2(_tmpro.rectTransform.rect.width, _tmpro.rectTransform.rect.height);

            pos.x += _tmpro.margin.x;
            size.x -= (_tmpro.margin.x + _tmpro.margin.z);
            Quaternion rotation = Quaternion.identity;
            for (int i = 0; i < info.characterCount; i++)
            {
                var charInfo = info.characterInfo[i];
                if (!charInfo.isVisible) continue;
                Vector3 center = (newVerts[charInfo.vertexIndex] + newVerts[charInfo.vertexIndex + 2]) / 2;
                if(_bendMode == BendMode.Character)
                {
                    float percent = Mathf.InverseLerp(pos.x, pos.x + size.x, center.x);
                    if(_stretchMode == StretchMode.Original)
                    {
                        percent = (float)TravelWithOffset(0f, center.x - pos.x, Spline.Direction.Forward, offset, out float moved);
                    }
                    Evaluate((double)percent, ref evalResult);
                    ModifySample(ref evalResult);
                    evalResult.position += evalResult.up * _offset.y + evalResult.right * _offset.x;
                    rotation = evalResult.rotation * Quaternion.AngleAxis(-90f, Vector3.up) * Quaternion.AngleAxis(_rotation + 90, Vector3.right);
                }

                for (int v = 0; v < 4; v++)
                {
                    int index = charInfo.vertexIndex + v;
                    if (_bendMode == BendMode.Distort)
                    {
                        float percent = Mathf.InverseLerp(pos.x, pos.x + size.x, newVerts[index].x);
                        if (_stretchMode == StretchMode.Original)
                        {
                            percent = (float)TravelWithOffset(0f, newVerts[index].x - pos.x, Spline.Direction.Forward, offset, out float moved);
                        }
                        Evaluate((double)percent, ref evalResult);
                        ModifySample(ref evalResult);
                        evalResult.position += evalResult.up * _offset.y + evalResult.right * _offset.x;
                        rotation = evalResult.rotation * Quaternion.AngleAxis(-90f, Vector3.up) * Quaternion.AngleAxis(_rotation + 90, Vector3.right);
                        newVerts[index] = evalResult.position + rotation * (Vector3.up * newVerts[index].y * evalResult.size);
                    } else
                    {
                        Matrix4x4 matrix = Matrix4x4.TRS(evalResult.position, rotation, Vector3.one * evalResult.size);
                        newVerts[index] = matrix.MultiplyPoint3x4(newVerts[index] - center + Vector3.up * center.y);
                    }
                    newNormals[index] = rotation * Vector3.back;
                    newColors[index] *= evalResult.color;
                }
            }
            _tmpro.mesh.vertices = newVerts;
            _tmpro.mesh.colors = newColors;
            _tmpro.mesh.normals = newNormals;
            MeshUtility.TransformMesh(_tmpro.mesh, _tmpro.transform.worldToLocalMatrix);
            _tmpro.mesh.RecalculateBounds();
            if (_tmpro is TextMeshProUGUI)
            {
                var uguiTmpro = (TextMeshProUGUI)_tmpro;
                uguiTmpro.UpdateGeometry(uguiTmpro.mesh, 0);
            }
        }
    }
}