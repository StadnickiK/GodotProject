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

        private List<T> _targets;
        public List<T> Targets
        {
            get { return _targets; }
        }
        

        public void SetTarget(T Target){
            _targets.Clear();
            _hasTarget = true;
            currentTarget = Target;
            _targets.Add(currentTarget);
        }

        public void AddTarget(T Target){
            if(!_hasTarget){
                SetTarget(Target);
            }else{
                _targets.Add(Target);
            }
        }

        public void NextTarget(){
            if(_targets.Count > 1){
                _targets.RemoveAt(0);
                currentTarget = _targets[0];
            }else{
                GD.Print("Clear");
                ClearTargets();
            }
        }

        public void ClearTargets(){
            currentTarget = default(T);
            _targets.Clear();
            _hasTarget = false;
        }
    }