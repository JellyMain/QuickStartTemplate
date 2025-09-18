using UnityEngine;


namespace CustomLayoutGroups
{
    public interface ILayoutElementMover
    {
        public void MoveElement(Vector3 targetPosition);
    }
}