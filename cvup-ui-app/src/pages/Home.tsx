import { useEffect, useState } from "react";
import { observer } from "mobx-react";
import { useStore } from "../Hooks/useStore";

export const Home: React.FC = observer(() => {
  const { authStore } = useStore();

  const [id, setId] = useState(0);

  useEffect(() => {}, []);

  return (
    <div>
      <div>{authStore.userName}</div>
    </div>
  );
});
