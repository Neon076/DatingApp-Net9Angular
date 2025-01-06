import { CanDeactivateFn } from '@angular/router';
import { EditMemberComponent } from '../members/edit-member/edit-member.component';
import { inject } from '@angular/core';
import { ConfirmService } from '../_services/confirm.service';

export const preventUnsavedChangesGuard: CanDeactivateFn<EditMemberComponent> = (component) => {
  const confirmService = inject(ConfirmService);
  if(component.editForm?.dirty){
    return confirmService.confirm() ?? false;
  }
  return true;
};
