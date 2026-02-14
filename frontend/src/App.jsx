import { useState } from "react";
import ClaimForm from "./ClaimForm";
import ClaimList from "./ClaimList";

function App() {
  const [reloadFlag, setReloadFlag] = useState(false);
  const [selectedClaim, setSelectedClaim] = useState(null);

  const handleSuccess = () => {
    setReloadFlag(prev => !prev);
    setSelectedClaim(null); // reset edit mode
  };

  return (
    <>
      <ClaimForm
        selectedClaim={selectedClaim}
        onSuccess={handleSuccess}
      />

      <ClaimList
        reloadFlag={reloadFlag}
        onEdit={setSelectedClaim}
      />
    </>
  );
}

export default App;
