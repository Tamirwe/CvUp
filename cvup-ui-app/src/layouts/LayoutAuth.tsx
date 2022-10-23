import { Outlet } from "react-router-dom";

export const LayoutAuth: React.FC = () => {
  return (
    <div>
      <div>Layout Auth</div>
      <Outlet />
    </div>
  );
};
