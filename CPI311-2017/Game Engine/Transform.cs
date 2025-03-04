﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace CPI311.GameEngine
{
    public class Transform : Component
    {
        private Vector3 localPosition;
        private Quaternion localRotation;
        private Vector3 localScale;
        private Matrix world;
        private List<IUpdateable> Updateables { get; set; }

        // Update for Parent
        private Transform parent;
        private List<Transform> Children { get; set; }

        public Vector3 LocalPosition
        {
            get { return localPosition; }
            set { localPosition = value; UpdateWorld(); }
        }

        public Quaternion LocalRotation
        {
            get { return localRotation; }
            set { localRotation = value; UpdateWorld(); }
        }

        public Vector3 LocalScale
        {
            get { return localScale; }
            set { localScale = value; UpdateWorld(); }
        }

        private void UpdateWorld()
        {
            world = Matrix.CreateScale(localScale) *
                Matrix.CreateFromQuaternion(localRotation) *
                Matrix.CreateTranslation(localPosition);

            if (parent != null)
                world *= parent.World;
            foreach (Transform child in Children)
                child.UpdateWorld();
        }

        public Transform()
        {
            localPosition = Vector3.Zero;
            localRotation = Quaternion.Identity;
            localScale = Vector3.One;
            Updateables = new List<IUpdateable>();

            Children = new List<Transform>();
            parent = null;
            UpdateWorld();
        }

        public Vector3 Forward
        {
            get { return world.Forward; }
        }

        public Vector3 Backward
        {
            get { return world.Backward; }
        }

        public Vector3 Up
        {
            get { return world.Up; }
        }

        public Vector3 Down
        {
            get { return world.Down; }
        }

        public Vector3 Right
        {
            get { return world.Right; }
        }

        public Vector3 Left
        {
            get { return world.Left; }
        }

        public void Rotate(Vector3 axis, float angle)
        {
            localRotation *= Quaternion.CreateFromAxisAngle(axis, angle);
            UpdateWorld();
        }

        public Vector3 Position
        {
            get { return world.Translation; }
            set
            {
                if (Parent == null)
                    LocalPosition = value;
                else
                {
                    Matrix total = World;
                    total.Translation = value;
                    LocalPosition = (Matrix.Invert(Matrix.CreateScale(LocalScale) * 
                        Matrix.CreateFromQuaternion(LocalRotation)) * total * Matrix.Invert(Parent.World)).Translation;
                }
            }
        }

        public Matrix World
        {
            get { return world; }
        }

        
        public Transform Parent
        {
            get { return parent; }
            set
            {
                if (parent != null)
                    parent.Children.Remove(this);
                parent = value;
                if (parent != null)
                    parent.Children.Add(this);
                UpdateWorld();
            }
        }

        // Updated for Lab #5
        public Quaternion Rotation
        {
            get { return Quaternion.CreateFromRotationMatrix(World); }
            set
            {
                if (Parent == null) LocalRotation = value;
                else
                {
                    Vector3 scale, pos;
                    Quaternion rot;
                    world.Decompose(out scale, out rot, out pos);
                    Matrix total = Matrix.CreateScale(scale) *
                          Matrix.CreateFromQuaternion(value) *
                          Matrix.CreateTranslation(pos);
                    LocalRotation = Quaternion.CreateFromRotationMatrix(
                         Matrix.Invert(Matrix.CreateScale(LocalScale)) * total *
                         Matrix.Invert(Matrix.CreateTranslation(LocalPosition)
                         * Parent.world));
                }
            }
        }

        // Update for Assignment #3
        public void Update()
        {
            foreach (IUpdateable component in Updateables)
                component.Update();

            UpdateWorld();
        }
    }
}
