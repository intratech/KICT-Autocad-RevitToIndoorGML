export class ArrayUtil {
    public groupBy(list, keyGetter) {
        const map = new Map();
        list.forEach((item) => {
             const key = keyGetter(item);
             const collection = map.get(key);
             if (!collection) {
                 map.set(key, [item]);
             } else {
                 collection.push(item);
             }
        });
        return map;
    }
    public static min(list, keySelector){
        return list.reduce((previousObj, currentOjb) => {
            let previousValue = keySelector(previousObj); 
            let currentValue = keySelector(currentOjb);

            if(currentValue < previousValue){
                return currentOjb;
            }
            else{
                return previousObj;
            }
        });
    }
    public static max(list, keySelector){
        return list.reduce((previousObj, currentOjb) => {
            let previousValue = keySelector(previousObj); 
            let currentValue = keySelector(currentOjb);

            if(currentValue > previousValue){
                return currentOjb;
            }
            else{
                return previousObj;
            }
        });
    }
}