import { Component, computed, inject, input, output } from '@angular/core';
import { Member } from '../../_models/members';
import { RouterLink } from '@angular/router';
import { LikesService } from '../../_services/likes.service';
import { PresenceService } from '../../_services/presence.service';

@Component({
  selector: 'app-member-card',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './member-card.component.html',
  styleUrl: './member-card.component.css'
})
export class MemberCardComponent {
  private likeService = inject(LikesService);
  private presenceService = inject(PresenceService);
  member = input.required<Member>();
  hasLiked = computed(()=> this.likeService.likeIds().includes(this.member().id))
  // computed because we dont need to update hasLiked just update likeIds()
  // and it will be computed and hasLiked will be update ... computed is a Signal
  isOnline = computed(()=> this.presenceService.onlineUsers().includes(this.member().username))
  toggleLike(){
    this.likeService.toggleLike(this.member().id).subscribe({
      next : () => {
        if(this.hasLiked()){
          this.likeService.likeIds.update(ids => ids.filter(x => x!== this.member().id))
        }else{
          this.likeService.likeIds.update(ids => [...ids , this.member().id])
        }
      }
    })
  }
}
