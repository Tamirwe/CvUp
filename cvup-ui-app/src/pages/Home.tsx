import { observer } from "mobx-react";
import { useEffect } from "react";
import { useStore } from "../Hooks/useStore";

export const Home: React.FC = observer(() => {
  const { authStore, generalStore } = useStore();

  useEffect(() => {
    generalStore.search();
  }, []);

  return (
    <div>
      <div>{authStore.claims.DisplayName}</div>
    </div>
  );
});
