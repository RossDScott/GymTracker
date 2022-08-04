import { exercisesAtom, selectedExerciseIndexAtom } from '../../shared/session.atoms';
import { useAtom, useAtomValue } from 'jotai';
import { selectAtom } from "jotai/utils";
import { ExerciseVM, SetMetrics } from '../../shared/session.model';
import AddExercise from './add-exercise';

const Exercises = () => {
    const exercises = useAtomValue(exercisesAtom);
    const [selectedIndex, setSelectedIndex] = useAtom(selectedExerciseIndexAtom);

    const handleSelectExercise = (e: React.MouseEvent, index: number) => {
        e.preventDefault();
        setSelectedIndex(index);
    }

    const items = exercises.map((exercise, index) =>
        <li  
            key={index}
            className={`list-group-item ${index === selectedIndex ? 'fw-bold' : ''}`}
            onClick={(e) => handleSelectExercise(e, index)}>
            {exercise.exercise.name}
        </li>
    );

    return (
        <>
            <ul className="list-group list-group-flush me-1" style={{overflowY: "auto", maxHeight:"85%"}}>
                {items}
            </ul>
            <AddExercise></AddExercise>
        </>
    )
}

export default Exercises;