# Unity Pool Manager

After reading a bunch of pool managers, I ended up picking on from a gist located here: https://gist.github.com/quill18/5a7cfffae68892621267 created by quill18

I've modified it to my needs and cleaned it up a bit and will keep modifying it as my needs dictate.

Usage is the same as his, a pool of the default size will be created for your prefab if you don't create the pool ahead of time.

Instead of calling Instantiate(), use this:
PoolManager.Spawn(somePrefab, somePosition, someRotation);
 
Instead of destroying an object, use this:
PoolManager.Despawn(myGameObject);

To create the pool ahead of time so you can set the size:
int poolSize = 10;
PoolManager.Init(somePrefab, poolSize);
