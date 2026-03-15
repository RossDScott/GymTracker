const DB_NAME = 'GymTrackerDB';
const DB_VERSION = 2;

let dbInstance = null;

function getDb() {
    if (dbInstance) return Promise.resolve(dbInstance);

    return new Promise((resolve, reject) => {
        const request = indexedDB.open(DB_NAME, DB_VERSION);

        request.onupgradeneeded = (event) => {
            const db = event.target.result;

            const stores = [
                { name: 'Exercises', keyPath: 'id' },
                { name: 'WorkoutPlans', keyPath: 'id' },
                { name: 'Workouts', keyPath: 'id', indexes: [{ name: 'workoutEnd', keyPath: 'workoutEnd' }] },
                { name: 'CurrentWorkout', keyPath: 'key' },
                { name: 'AppSettings', keyPath: 'key' },
                { name: 'ExerciseStatistics', keyPath: 'exerciseId' },
                { name: 'WorkoutPlanStatistics', keyPath: 'workoutPlanId' },
                { name: 'WorkoutStatistics', keyPath: 'workoutId', indexes: [{ name: 'workoutPlanId', keyPath: 'workoutPlanId' }] },
                { name: 'HasInitialisedDefaultData', keyPath: 'key' },
                { name: 'Meta', keyPath: 'key' },
                { name: 'NextWorkoutSummary', keyPath: 'key' }
            ];

            for (const store of stores) {
                if (!db.objectStoreNames.contains(store.name)) {
                    const os = db.createObjectStore(store.name, { keyPath: store.keyPath });
                    if (store.indexes) {
                        for (const idx of store.indexes) {
                            os.createIndex(idx.name, idx.keyPath, { unique: false });
                        }
                    }
                }
            }
        };

        request.onsuccess = (event) => {
            dbInstance = event.target.result;
            resolve(dbInstance);
        };

        request.onerror = (event) => {
            reject(event.target.error);
        };
    });
}

function txStore(storeName, mode) {
    return getDb().then(db => {
        const tx = db.transaction(storeName, mode);
        return tx.objectStore(storeName);
    });
}

function promisifyRequest(request) {
    return new Promise((resolve, reject) => {
        request.onsuccess = () => resolve(request.result);
        request.onerror = () => reject(request.error);
    });
}

export async function get(storeName, key) {
    const store = await txStore(storeName, 'readonly');
    const result = await promisifyRequest(store.get(key));
    return result || null;
}

export async function getAll(storeName) {
    const store = await txStore(storeName, 'readonly');
    const result = await promisifyRequest(store.getAll());
    return result || [];
}

export async function put(storeName, value) {
    const store = await txStore(storeName, 'readwrite');
    await promisifyRequest(store.put(value));
}

export async function putMany(storeName, items) {
    const db = await getDb();
    const tx = db.transaction(storeName, 'readwrite');
    const store = tx.objectStore(storeName);
    for (const item of items) {
        store.put(item);
    }
    return new Promise((resolve, reject) => {
        tx.oncomplete = () => resolve();
        tx.onerror = () => reject(tx.error);
    });
}

export async function del(storeName, key) {
    const store = await txStore(storeName, 'readwrite');
    await promisifyRequest(store.delete(key));
}

export async function clear(storeName) {
    const store = await txStore(storeName, 'readwrite');
    await promisifyRequest(store.clear());
}

export async function count(storeName) {
    const store = await txStore(storeName, 'readonly');
    return await promisifyRequest(store.count());
}

export async function getByIndex(storeName, indexName, key) {
    const store = await txStore(storeName, 'readonly');
    const index = store.index(indexName);
    const result = await promisifyRequest(index.getAll(key));
    return result || [];
}
