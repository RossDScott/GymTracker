import { ReactElement } from "react";

export type ButtonProps = {
    isPaused: boolean;
    isRunning: boolean;
    canPause: boolean;

    onStart: () => void;
    onPause: () => void;
    onReset: () => void;
}

const ControlButtons = (props: ButtonProps) => {
    return (
        <>
            {!props.isPaused && props.isRunning &&
                <button  
                    type="button" 
                    className="btn btn-outline-primary btn-sm ms-auto pt-0 pb-0"
                    onClick={props.onPause}>Pause</button>
            }
    
            {props.isPaused &&
                <button  
                    type="button" 
                    className="btn btn-outline-primary btn-sm ms-auto me-1 pt-0 pb-0"
                    onClick={props.onReset}>Reset</button>
            }
    
            {!props.isRunning && 
                <button 
                    type="button" 
                    className={`btn btn-outline-primary btn-sm pt-0 pb-0 ${props.isPaused ? 'ms-1' : 'ms-auto'}`}
                    onClick={props.onStart}>{props.isPaused ? 'Resume' : 'Start'}</button>
            }
        </>
    );
}

export default ControlButtons;


