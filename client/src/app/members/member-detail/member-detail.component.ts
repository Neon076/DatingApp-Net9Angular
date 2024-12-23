import { Component, inject, OnInit } from '@angular/core';
import { MembersService } from '../../_services/members.service';
import { ActivatedRoute } from '@angular/router';
import { Member } from '../../_models/members';
import { TabsModule } from 'ngx-bootstrap/tabs';
// import { GalleryModule, GalleryItem, ImageItem } from 'ng-gallery';
@Component({
  selector: 'app-member-detail',
  standalone: true,
  imports: [TabsModule],
  templateUrl: './member-detail.component.html',
  styleUrl: './member-detail.component.css'
})
export class MemberDetailComponent implements OnInit{
  private memberService = inject(MembersService);
  private route = inject(ActivatedRoute)
  member?: Member;  

  ngOnInit(): void {
    this.loadMember();
  }

  loadMember(){
    console.log("load Members");
   const username = this.route.snapshot.paramMap.get('username')
   if(!username) return;

   this.memberService.getMember(username).subscribe({
    next : member => this.member = member
   })
  }

}
