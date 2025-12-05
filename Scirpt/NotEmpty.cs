// if (prevIsGrounded != isGrounded && jumpState == JumpState.Grounded)
// {
//     jumpState = JumpState.Descending;
//     animator.SetTrigger(DESCENDING);
//     return;
// }
//
//
// switch (jumpState)
// {
//     case JumpState.Grounded:
//         if (Input.GetKeyDown(KeyCode.Space))
//         {
//             jumpState = JumpState.Ascending;
//             startPosition = transform.position;
//             animator.SetTrigger(JUMP);
//         }
//     break;
//     case JumpState.Ascending:
//         if (transform.position.y >= startPosition.y + jumpHeight)
//         {
//             jumpState = JumpState.Descent;
//             return;
//         }
//         MoveY(ascendingSpeed);
//         break;
//     case JumpState.Descending:
//         if (isGrounded)
//         {
//             jumpState = JumpState.Grounded;
//             return;
//         }
//         MoveY(-descendingSpeed);
//         break;
//     default:
//         break;
// }
//
// void MoveY(float ySpeed)
// {
//     float newY = transform.position.y + (ySpeed * Time.deltaTime);
//     transform.position = new Vector3(transform.position.x, newY, transform.position.z);
// }