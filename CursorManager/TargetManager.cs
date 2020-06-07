using Godot;
using System;
using System.Collections.Generic;

    public class TargetManager{

        public KinematicCube currentTarget = null;
        List<KinematicCube> targets = new List<KinematicCube>();

        public bool HasTarget(){
            if(currentTarget != null){
                return true;
            }
            return false;
        }
        public void SetTarget(KinematicCube Target){
            currentTarget = Target;
            targets.Add(currentTarget);
            targets.Clear();
        }

        public void AddTarget(KinematicCube Target){
            targets.Add(Target);
        }

        public void ClearTargets(){
            currentTarget = null;
            targets.Clear();
        }
    }