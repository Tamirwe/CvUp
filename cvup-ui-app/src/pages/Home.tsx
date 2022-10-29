import { observer } from "mobx-react";
import { useStore } from "../Hooks/useStore";

export const Home: React.FC = observer(() => {
  const { authStore } = useStore();

  return (
    <div>
      <div>{authStore.claims.DisplayName}</div>
    </div>
  );
});
