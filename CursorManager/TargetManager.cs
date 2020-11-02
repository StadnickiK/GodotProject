using Godot;
using System;
using System.Collections.Generic;

    public class TargetManager<T> : Node{

        public T currentTarget = default(T);

        private bool _hasTarget = false;
        public bool HasTarget
        {
            get { return _hasTarget; }
        }

        public override void _Ready()
        {    
            SetProcess(false);
        }

        List<T> targets = new List<T>();

        public void SetTarget(T Target){
            targets.Clear();
            _hasTarget = true;
            currentTarget = Target;
            targets.Add(currentTarget);
        }

        public void AddTarget(T Target){
            if(!_hasTarget){
                SetTarget(Target);
            }else{
                targets.Add(Target);
            }
        }

        public void NextTarget(){
            if(targets.Count > 1){
                targets.RemoveAt(0);
                currentTarget = targets[0];
            }else{
                GD.Print("Clear");
                ClearTargets();
            }
        }

        public void ClearTargets(){
            currentTarget = default(T);
            targets.Clear();
            _hasTarget = false;
        }
    }