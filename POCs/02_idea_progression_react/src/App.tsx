import './App.css';
import { Provider } from 'jotai';
import Session from './session/session';

const App = () => (
    <Provider>
        <Session></Session>
    </Provider>
)

export default App;
