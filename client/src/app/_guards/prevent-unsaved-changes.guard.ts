import { CanDeactivateFn } from '@angular/router';
import { EditMemberComponent } from '../members/edit-member/edit-member.component';

export const preventUnsavedChangesGuard: CanDeactivateFn<EditMemberComponent> = (component) => {

  if(component.editForm?.dirty){
    return confirm('Are you sure you want to continue? Any unsaved changes will be lost.')
  }
  return true;
};
