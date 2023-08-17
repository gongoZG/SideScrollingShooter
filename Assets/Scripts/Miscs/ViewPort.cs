using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewPort : Singleton<ViewPort>
{
    float minX, middleX, maxX, minY, maxY;
    public float MaxX => maxX;

    private void Start() {
        Camera mainCamera = Camera.main;
        // 需要将视口坐标转化为世界坐标才能正确限制玩家的位置
        Vector2 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0f, 0f));
        Vector2 topRight = mainCamera.ViewportToWorldPoint(new Vector3(1f, 1f));
        middleX = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0f, 0f)).x;

        minX = bottomLeft.x;
        maxX = topRight.x;
        minY = bottomLeft.y;
        maxY = topRight.y;
    }

    public Vector3 PlayerMoveablePosition(Vector3 playerPosition, 
        float paddingX, float paddingY) {
        Vector3 position = Vector3.zero;
        position.x = Mathf.Clamp(playerPosition.x, minX + paddingX, maxX - paddingX);
        position.y = Mathf.Clamp(playerPosition.y, minY + paddingY, maxY - paddingY);
        return position;
    }

    public Vector3 RandomEnemySpawnPosition(float paddingX, float paddingY) {
        return new Vector3(
            maxX + paddingX, 
            Random.Range(minY + paddingY, maxY - paddingY)
        );
    }

    public Vector3 RandomRightHalfPosition(float paddingX, float paddingY) {
        return new Vector3(
            Random.Range(middleX, maxX - paddingX),
            Random.Range(minY + paddingY, maxY - paddingY)
        );
    }

}
