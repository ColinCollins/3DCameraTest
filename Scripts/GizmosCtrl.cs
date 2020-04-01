using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmosCtrl : MonoBehaviour
{
    public LayerMask Layer;
    public GizmosObj[] Arrows;
    public bool isDragingGizmos = true;

    private Vector3 startPos;
    private Vector3 curPos;
    private Vector3 originLocalPos;

    private GizmosObj curGizmos;

    public float MoveScale = 50f;

    public void Start()
    {
        for (int i = 0; i < Arrows.Length; i++)
        {
            Arrows[i].Init();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            for (int i = 0; i < Arrows.Length; i++) {
                Arrows[i].SwtichMatColor(false);
            }

            isDragingGizmos = false;
            curGizmos = null;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit info;

            if (Physics.Raycast(ray, out info, 100, Layer))
            {
                isDragingGizmos = true;
                curGizmos = info.transform.GetComponent<GizmosObj>();
                curGizmos.SwtichMatColor(true);

                startPos = Input.mousePosition;
                originLocalPos = transform.localPosition;
            }
        }

        if (Input.GetMouseButton(0) && isDragingGizmos) 
        {
            curPos = Input.mousePosition;
            Vector3 dp = curPos - startPos;
            var tmpPos = originLocalPos;

            switch (curGizmos.Dir)
            {
                case Direction.X:
                    tmpPos.x += dp.x / MoveScale;
                    transform.localPosition = tmpPos;
                    break;
                case Direction.Y:
                    tmpPos.y += dp.y / MoveScale;
                    transform.localPosition = tmpPos;
                    break;
                case Direction.Z:
                    tmpPos.z += dp.z / MoveScale;
                    transform.localPosition = tmpPos;
                    break;
                default:
                    break;
            }
        }
       
    }
}
