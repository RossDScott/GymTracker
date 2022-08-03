import { selectedExerciseAtom, sessionAtom } from '../../shared/session.atoms';
import { useAtom, useAtomValue } from 'jotai';
import { selectAtom } from "jotai/utils";
import { ExerciseVM, SetMetrics } from '../../shared/session.model';
import AddExercise from './add-exercise';

const exercisesAtom = selectAtom(sessionAtom, session => session.exercises);

const Exercises = () => {
    const exercises = useAtomValue(exercisesAtom);
    const [selectedExercise, setSelectedExercise] = useAtom(selectedExerciseAtom);

    const handleSelectExercise = (e: React.MouseEvent, exercise: ExerciseVM<SetMetrics>) => {
        e.preventDefault();
        setSelectedExercise(exercise);
    }

    const items = exercises.map((exercise, index) =>
        <li  
            key={index}
            className={`list-group-item ${selectedExercise === exercise ? 'fw-bold' : ''}`}
            onClick={(e) => handleSelectExercise(e, exercise)}>
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