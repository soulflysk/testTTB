import { useState } from 'react'
import ClaimForm from './ClaimForm'
import ClaimList from './ClaimList'
import './App.css'

function App() {
const [reloadFlag, setReloadFlag] = useState(false);

  const handleSuccess = () => {
    setReloadFlag(prev => !prev); // toggle เพื่อ trigger useEffect
  };
  return (
    <>
      <ClaimForm onSuccess={handleSuccess} />
      <ClaimList reloadFlag={reloadFlag} />
    </>
  )
}

export default App
