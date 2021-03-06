﻿namespace Gridia
{
    using System;

    using UnityEngine;

    public class Layer
    {
        #region Fields

        public readonly Func<Tile, int> GetHeight;
        public readonly Func<Tile, Vector2> GetOffset;
        public readonly Func<int, Texture> GetTexture;
        public readonly Func<Tile, int> GetTileData;
        public readonly Func<Tile, int> GetWidth;
        public readonly Mesh Mesh;
        public readonly GameObject Renderable;
        public readonly Vector2[] uv;

        private readonly TileMapView _view;

        #endregion Fields

        #region Constructors

        public Layer(
            string name,
            TileMapView view,
            Func<Tile, int> getTileData,
            Func<int, Texture> getTexture,
            Func<Tile, Vector2> getOffset = null,
            Func<Tile, int> getWidth = null,
            Func<Tile, int> getHeight = null)
        {
            _view = view;
            GetTileData = getTileData;
            GetTexture = getTexture;
            GetOffset = getOffset ?? (tile => Vector2.zero);
            GetWidth = getWidth ?? (tile => 1);
            GetHeight = getHeight ?? (tile => 1);
            Renderable = InitRenderable(name);
            Mesh = Renderable.GetComponent<MeshFilter>().mesh; //smell
            uv = InitUV();
        }

        #endregion Constructors

        #region Methods

        public void ApplyUV()
        {
            Mesh.uv = uv;
        }

        public void Delete()
        {
            UnityEngine.Object.Destroy(Renderable);
        }

        private GameObject InitRenderable(string name)
        {
            var renderable = new GameObject(name, typeof(MeshRenderer), typeof(MeshFilter));
            renderable.GetComponent<MeshFilter>().mesh = new Mesh();
            //renderable.transform.parent = GameObject.Find("Game").transform;
            //renderable.transform.localScale = Vector3.one; // :(
            return renderable;
        }

        private Vector2[] InitUV()
        {
            var uv = new Vector2[_view.NumTiles * 4];
            for (var i = 0; i < uv.Length; i++)
            {
                uv[i] = new Vector2();
            }
            return uv;
        }

        #endregion Methods
    }
}