using Godot;
using System;
using System.Collections.Generic;


    


    public partial class TargetManager<T> : Node{

        public struct Target
        {
            public Target(Vector3 vector3,T obj = default) { TargetNode = obj; Point = vector3; }
            public Vector3 Point { get; set; }

            public T TargetNode { get; set; }
        }  

        public Target currentTarget;  

        public delegate void AtTargetEventHandeler(T node);

        public event AtTargetEventHandeler AtTarget;


        public override void _Ready()
        {    
            Name = "TargetManager";
            SetProcess(false);
        }

        private Stack<Target> _targets = new Stack<Target>();
        public Stack<Target> Targets
        {
            get { return _targets; }
        }

        public bool HasTarget
        {
            get { return Targets.Count>0; }
        }
        

        public void SetTarget(Target Target){
            _targets.Clear();
            currentTarget = Target;
            _targets.Push(currentTarget);
        }

        public void AddTarget(Target Target){
            if(!HasTarget){
                SetTarget(Target);
            }else{
                _targets.Push(Target);
            }
        }

        public void NextTarget(){
            if(_targets.Count > 1){
                AtTarget?.Invoke(currentTarget.TargetNode);
                _targets.Pop();
                currentTarget = _targets.Peek();
            }else{
                ClearTargets();
                currentTarget = default(Target);
            }
        }

        public void ClearTargets(){
            currentTarget = default(Target);
            _targets.Clear();
        }
    }

