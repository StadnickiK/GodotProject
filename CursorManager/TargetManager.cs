using Godot;
using System;
using System.Collections.Generic;

    public class TargetManager<T>{

        public T currentTarget = default(T);

        private bool _hasTarget = false;
        public bool HasTarget
        {
            get { return _hasTarget; }
        }

        List<T> targets = new List<T>();

        public void SetTarget(T Target){
            _hasTarget = true;
            currentTarget = Target;
            targets.Clear();
            targets.Add(currentTarget);
        }

        public void AddTarget(T Target){
            if(!_hasTarget){
                SetTarget(Target);
            }else{
                targets.Add(Target);
            }
        }

        public void ClearTargets(){
            currentTarget = default(T);
            targets.Clear();
            _hasTarget = false;
        }
    }