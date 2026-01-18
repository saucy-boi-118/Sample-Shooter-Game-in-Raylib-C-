
using System;
using Raylib_cs;
using System.Collections.Generic;
using System.Numerics;




class Utilities
{
    // return true if equal false if not
    public static bool IsEqualP(Vector2 A, Vector2 B)
    {
        if (A.X == B.X) return true;
        if (A.Y == B.Y) return true;
        return false;
    }

    public static void SpawnE(List<Vector3> enemies, int threshold, Vector2 cellNum, int cellSize)
    {
        if (enemies.Count < threshold)
        {
            Vector3 enemy = new(cellNum.X*cellSize, 0.0f, cellNum.Y*cellSize);
            enemies.Add(enemy);
        }
    }

    // just in case the first method didnt work
        //v.X += 2 * (dx/10);
        //v.Z += 2 * (dz/10);
    public static Vector3 FollowP(Vector3 target, Vector3 follower, float dx, float dz, double follow)
    {

        Vector3 v = follower;
        dx = target.X - v.X;
        dz = target.Z - v.Z;


        follow = Math.Atan2(dz, dx);

        //Console.Clear();
        v.X += (float)(Math.Cos(follow));
        v.Z += (float)(Math.Sin(follow));

        return v;

        
    }

    public static Vector3 MoveBullet(Vector3 bullet, int speed, float angle) //float pitch (vertical shooting)
    {
        Vector3 b = bullet;

        angle *= (float)(-Math.PI/180);
        //pitch *= (float)(-Math.PI/180);

        


        b.X += (float)(-Math.Sin(angle) * speed);
        b.Z += (float)(-Math.Cos(angle) * speed);
        //b.Y += (float)(-Math.Cos(angle) * speed);

        return b;
    }
    public static void Shoot(List<Vector3> bullets, Vector3 player, int threshold)
    {
        if (bullets.Count < threshold)
        {
            Vector3 bullet = player;
            bullets.Add(bullet);
        }
    }

    public static float DistanceV(Vector3 A, Vector3 B)
    {
        // find the distance between two vectors on the X and Z plane
        
        return (float)(Math.Sqrt(Math.Pow(A.X - B.X, 2) + Math.Pow(A.Z - B.Z, 2)));
    }
}

class Game
{
    
    public static void Main(string[] args)
    {
        // INIT SCREEN
            Raylib.InitWindow(1000, 500, "3D Shooter");

            //CAMERA
            Camera3D camera = new()
            {
                Position = new Vector3(0.0f, 0.0f, 25.0f),
                Target = new Vector3(0.0f, 0.0f, 0.0f),
                Up = new Vector3(0.0f, 10.0f, 0.0f),
                FovY = 60.0f,
                Projection = CameraProjection.Perspective

            };
            // better visibility
            Raylib.DisableCursor();

            //Target FPS
            Raylib.SetTargetFPS(60);

            // GAME VARIABLES

            //enemies
            int maxE = 5;
            List<Vector3> enemies = [];
            //int enemyHp = 5;


            // player
            Vector3 playerPos = new(0.0f, 10.0f, 0.0f);
            Vector3 playerSize = new(10, 10, 10); // take out later replace with texture\
            //movement speed and rotation speed variables
            Vector3 movementV = new(0.1f, 0.1f, 0.0f);
            float movementS = 5.0f;
            Vector3 rotationV = new(0.0f, 0.0f, 0.0f);
            int rotationS = 5;
            //gamepad
            float gx;
            float gy;
            //cell variables
            int cellSize = 50;
            Vector2 prevCell = new((float)Math.Round(camera.Position.X/cellSize), (float)Math.Round(camera.Position.Z/cellSize));
            Vector2 curCell = prevCell;
            // enemy follow
            float dx = 0.0f;
            float dz = 0.0f;
            double follow = 0;
            //shooting
            

            // gun
            Vector3 vectorG = new(camera.Position.X + 5, camera.Position.Y, camera.Position.Z - 10);


            //int bulletHp = 5;
            List<Vector3> bullets = [];
            float yaw = 0.0f;
            //float pitch = 0.0f;
            
            // Map
            Model map = Raylib.LoadModel("Assets/map.glb");
            Vector3 mapPos = new(0.0f, -10.0f, 0.0f);
            Texture2D mapTex = Raylib.LoadTexture("Snow004.png");
            Raylib.SetMaterialTexture(ref map, 0, MaterialMapIndex.Diffuse, ref mapTex);
            

           

            while (!Raylib.WindowShouldClose())
            {
                // update cell
                curCell = new((float)Math.Round(camera.Position.X/cellSize), (float)Math.Round(camera.Position.Z/cellSize));
                if (!Utilities.IsEqualP(curCell, prevCell))
                {
                    prevCell = curCell; 
                    Utilities.SpawnE(enemies, maxE, curCell, cellSize);  
                    
                } 

                //gun
                vectorG = new(camera.Position.X + 5, camera.Position.Y, camera.Position.Z - 10);
                //check


                //movement
                if (Raylib.IsGamepadAvailable(0) == true)
                {
                    //looking around on gamepad
                    rotationV.X = Raylib.GetGamepadAxisMovement(0,GamepadAxis.RightX) * rotationS;
                    rotationV.Y = Raylib.GetGamepadAxisMovement(0,GamepadAxis.RightY) * rotationS;

                    //move on gamepad left joystick
                    movementV = new(0.0f, 0.0f, 0.0f);
                    gx = Raylib.GetGamepadAxisMovement(0,GamepadAxis.LeftX);
                    gy = Raylib.GetGamepadAxisMovement(0,GamepadAxis.LeftY);
                    if(Math.Round(Math.Abs(gx)) != 0)movementV.X += movementS;
                    if(Math.Round(Math.Abs(gy)) != 0)movementV.Y += -movementS;
                } else
                {
                    // rotation on mouse, divided by 10 to reduce sensitivity
                    rotationV.X = Raylib.GetMouseDelta().X * rotationS/10;
                    rotationV.Y = Raylib.GetMouseDelta().Y * rotationS/10;
                    
                    yaw += (float)Raylib.GetMouseDelta().X/130 * 90;
                    //pitch += (float)Raylib.GetMouseDelta().Y/20 * 90;
                    //if (Math.Abs(yaw) > 360) yaw = 0;
                    if (yaw >= 360) yaw -=360;
                    if (yaw <= -360) yaw += 360;
                    //if (pitch > 180) pitch -= 180;
                    //if (pitch <= -180) pitch += 180;

                   
                    
                    // move on normal controls WASD

                    //Reset movement vector
                    movementV = new(0.0f, 0.0f, 0.0f);
                    //divide by smaller number to go faster
                    if (Raylib.IsKeyDown(KeyboardKey.W)) movementV.X += movementS/4;
                    if (Raylib.IsKeyDown(KeyboardKey.D)) movementV.Y += movementS/4;
                    // negative for reverse movement
                    if (Raylib.IsKeyDown(KeyboardKey.S)) movementV.X += -movementS/4;
                    if (Raylib.IsKeyDown(KeyboardKey.A)) movementV.Y += -movementS/4;
                }
                
                //Updating camera
                Raylib.UpdateCamera(ref camera, CameraMode.FirstPerson);
                Raylib.UpdateCameraPro( ref camera, movementV, rotationV, 0.0f);
                

                //Shooting
                if (bullets.Count < maxE && Raylib.IsMouseButtonDown(MouseButton.Left))
                {
                    Utilities.Shoot(bullets, camera.Position, maxE);                
                }
                 


                




                //BEGIN DRAWING--------------------------------------------------------------------------------------
                Raylib.BeginDrawing();
                
                Raylib.ClearBackground(Color.White);

                Raylib.BeginMode3D(camera);
                
                //spawn
                Raylib.DrawCubeWiresV(playerPos, playerSize, Color.Blue);
                Raylib.DrawSphereWires(playerPos, playerSize.X/5, 5, 10, Color.Red);


                //map
                Raylib.DrawModel(map, mapPos, 3.5f, Color.White);
                

                //gun
                Raylib.DrawCube(vectorG, 2.5f, 5.0f, 10.0f, Color.Black);

                


                    //load enemies
                    for (int i = 0; i < enemies.Count; i++)
                    {
                        if (Utilities.DistanceV(camera.Position, enemies[i]) < cellSize*2 )
                        {
                            enemies[i] = Utilities.FollowP(camera.Position, enemies[i], dx, dz, follow);
                        }
                            Raylib.DrawCubeWiresV(enemies[i], playerSize, Color.Black);   
                    }

                    //load bullets
                    for (int i = 0; i < bullets.Count; i++)
                    {
                        if (Utilities.DistanceV(camera.Position, bullets[i]) < cellSize*2)
                        {
                        bullets[i] = Utilities.MoveBullet(bullets[i], 5, yaw);
                    
                        Raylib.DrawSphereWires(bullets[i], 5.0f, 5, 10, Color.Red);
                        } else
                        {
                        bullets.Remove(bullets[i]);
                        }
                    }
            

           

                Raylib.EndMode3D();
                // END DRAWING---------------------------------------------------------------------------------------
                Raylib.EndDrawing();
            }

            //unload textures
           Raylib.UnloadModel(map);
           Raylib.UnloadTexture(mapTex);


            // UN - INIT WINDOW
            Raylib.CloseWindow();
    }
}

    
        

        
