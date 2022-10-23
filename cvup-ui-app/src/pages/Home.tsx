import { observer } from "mobx-react";
import { useStore } from "../Hooks/useStore";

export const Home: React.FC = observer(() => {
  const { authStore } = useStore();

  return (
    <div>
      <div>{authStore.userName}</div>
    </div>
  );
});
