export function useFormState<T>(args: T): [T] {
  // const [formModel, setFormModel] = useState<T>(args);
  // const [formValError, setFormValError] = useState<[{}]>();

  let key = typeof args;
  type staffKeys = keyof T;

  // console.log(typeof args);

  for (const key in args) {
    // Get the indexed item by the key:
    const indexedItem = args[key];
    console.log(indexedItem);
    // Now we have the item.

    // Use it...
  }

  //   const [value, setValue] = useState<T | undefined>(args);

  return [args];
}

//  const useFormState=(args:Type):string=>{

//     return "";
//  }
