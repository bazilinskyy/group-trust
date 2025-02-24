/*
 * This code is part of Arcade Car Physics for Unity by Saarg (2018)
 *
 * This is distributed under the MIT Licence (see LICENSE.md for details)
 */

using System.Collections.Generic;
using UnityEngine;


namespace VehicleBehaviour.Trails
{
    // Created by Edward Kay-Coles a.k.a Hoeloe
    public class Trail
    {
        //Properties of the trail
        private readonly float width;
        private readonly float decay;
        private readonly Material m;
        private int rough;
        private readonly int maxRough;
        private readonly bool softSource;

        //Parent object
        private readonly Transform par;

        //Pieces for the mesh generation
        private readonly GameObject trail;
        private readonly MeshFilter filter;
        private readonly MeshRenderer render;
        private readonly Mesh mesh;

        //Lists storing the mesh data
        private readonly LinkedList<Vector3> verts = new();
        private readonly LinkedList<Vector2> uvs = new();
        private readonly LinkedList<int> tris = new();
        private readonly LinkedList<Color> cols = new();

        //Check if the trail is still being generated, and if it has completely faded
        private bool finished = false;
        private bool dead = false;

        private Vector3 previousPosition;
        private Vector3 currentPosition;
        public float minimumSegmentLength = 0.1f;
        public Vector3 positionOffset;


        //Set up the trail object, and parameters
        public Trail(Transform parent, Material material, float decayTime, int roughness, bool softSourceEdges, Vector3 off, float wid = 0.1f)
        {
            softSource = softSourceEdges;
            maxRough = roughness;
            rough = 0;
            decay = decayTime;
            par = parent;
            width = wid;
            m = material;
            trail = new GameObject("Trail");
            filter = trail.AddComponent(typeof(MeshFilter)) as MeshFilter;
            render = trail.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
            mesh = new Mesh();
            render.material = m;
            filter.mesh = mesh;
            positionOffset = off;
        }


        //For registering if the object has been removed from the game (so you don't have to store it any more)
        public bool Dead
        {
            get => dead;
            private set
            {
                dead = true;
                Object.Destroy(trail);
            }
        }

        //Tells you if the trail is emitting or not
        public bool Finished => finished;


        //Call this when the trail should stop emitting
        public void Finish()
        {
            finished = true;
        }


        // Updates the state of the trail - Note: this must be called manually
        public void Update()
        {
            if (!finished) //Only add new segments if the trail is not being emitted
            {
                //Decides how often to generate new segments. Smaller roughness values are smoother, but more expensive
                if (rough > 0)
                {
                    rough--;
                }
                else
                {
                    rough = maxRough;

                    var currentPosition = par.transform.position + positionOffset;

                    if (Vector3.Distance(previousPosition, currentPosition) > minimumSegmentLength)
                    {
                        previousPosition = currentPosition;
                        //Add new vertices as the current position
                        var offset = par.right * width / 2f;
                        verts.AddLast(currentPosition - offset);
                        verts.AddLast(currentPosition + offset);

                        //Fades out the newest vertices if soft source edges is set to true
                        if (softSource)
                        {
                            if (cols.Count >= 4)
                            {
                                cols.Last.Value = Color.white;
                                cols.Last.Previous.Value = Color.white;
                            }

                            cols.AddLast(Color.clear);
                            cols.AddLast(Color.clear);
                        }
                        else
                        {
                            //Sets the first vertices to fade out, but leaves the rest solid
                            if (cols.Count >= 2)
                            {
                                cols.AddLast(Color.white);
                                cols.AddLast(Color.white);
                            }
                            else
                            {
                                cols.AddLast(Color.clear);
                                cols.AddLast(Color.clear);
                            }
                        }

                        uvs.AddLast(new Vector2(0, 1));
                        uvs.AddLast(new Vector2(1, 1));

                        //Don't try to draw the trail unless we have at least a rectangle
                        if (verts.Count < 4)
                        {
                            return;
                        }

                        //Add new triangles to the mesh
                        var c = verts.Count;
                        tris.AddLast(c - 1);
                        tris.AddLast(c - 2);
                        tris.AddLast(c - 3);
                        tris.AddLast(c - 3);
                        tris.AddLast(c - 2);
                        tris.AddLast(c - 4);

                        //Copy lists to arrays, ready to rebuild the mesh
                        var v = new Vector3[c];
                        var uv = new Vector2[c];
                        var t = new int[tris.Count];
                        verts.CopyTo(v, 0);
                        uvs.CopyTo(uv, 0);
                        tris.CopyTo(t, 0);

                        //Build the mesh
                        mesh.vertices = v;
                        mesh.triangles = t;
                        mesh.uv = uv;
                    }
                }
            }

            //The next section updates the colours in the mesh
            var i = cols.Count;

            //If we have no vertices, don't bother trying to update
            if (i == 0)
            {
                return;
            }

            //This is for checking if the trail has completely faded or not
            var alive = false;

            //Essentially a foreach loop over the colours, but allowing editing to each node as it goes
            var d = cols.First;

            do
            {
                if (d.Value.a > 0)
                {
                    var t = d.Value;
                    alive = true;
                    //Decrease the alpha value, to 0 if it would be decreased to negative
                    t.a -= Mathf.Min(Time.deltaTime / decay, t.a);
                    d.Value = t;
                }

                d = d.Next;
            } while (d != null);

            //Trail should be removed if it is not emitting and has faded out
            if (!alive && finished)
            {
                Dead = true;
            }
            else
            {
                //Doesn't set the colours if the number of vertices doesn't match up for whatever reason
                if (i != mesh.vertices.Length)
                {
                    return;
                }

                //Copy the colours to an array and build the mesh colours
                var cs = new Color[i];
                cols.CopyTo(cs, 0);
                mesh.colors = cs;
            }
        }
    }
}