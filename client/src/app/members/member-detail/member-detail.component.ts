import {
  Component,
  computed,
  Inject,
  inject,
  OnDestroy,
  OnInit,
  ViewChild,
} from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Member } from '../../_models/members';
import { TabDirective, TabsetComponent, TabsModule } from 'ngx-bootstrap/tabs';
import { MemberMessagesComponent } from '../member-messages/member-messages.component';
import { MessageService } from '../../_services/message.service';
import { CarouselModule } from 'ngx-bootstrap/carousel';
import { AccountService } from '../../_services/account.service';
import { HubConnectionState } from '@microsoft/signalr';
import { PresenceService } from '../../_services/presence.service';
import { LikesService } from '../../_services/likes.service';
import { TimeagoModule} from 'ngx-timeago';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-member-detail',
  standalone: true,
  imports: [TabsModule, MemberMessagesComponent, CarouselModule,TimeagoModule , DatePipe],
  templateUrl: './member-detail.component.html',
  styleUrl: './member-detail.component.css',
})
export class MemberDetailComponent implements OnInit, OnDestroy {
  @ViewChild('memberTabs', { static: true }) memberTabs?: TabsetComponent;
  // static true indicates that this memberTabs will initail when ngOnInit runs otherwise it will
  // initaill after the View is completed and that is after ngOnInit
  private messageService = inject(MessageService);
  private accountService = inject(AccountService);
  private route = inject(ActivatedRoute);
  private presenceService = inject(PresenceService);
  likesService = inject(LikesService);
  private router = inject(Router);
  member: Member = {} as Member;
  activateTab?: TabDirective;
  isOnline = computed(() =>
    this.presenceService.onlineUsers().includes(this.member.username)
  );
  hasLiked = computed(() =>
    this.likesService.likeIds().includes(this.member.id)
  );

  ngOnInit(): void {
    this.route.data.subscribe({
      next: (data) => {
        this.member = data['member'];
      },
    });

    this.route.paramMap.subscribe({
      next: () => this.onRouteParamsChange(),
    });
    this.route.queryParams.subscribe({
      next: (params) => {
        params['tab'] && this.selectTab(params['tab']);
      },
    });
  }

  selectTab(heading: string) {
    if (this.memberTabs) {
      const messageTab = this.memberTabs.tabs.find((x) => x.heading == heading);
      if (messageTab) messageTab.active = true;
    }
  }
  toggleLike() {
    this.likesService.toggleLike(this.member.id).subscribe({
      next: () => {
        if (this.hasLiked()) {
          this.likesService.likeIds.update((ids) =>
            ids.filter((x) => x !== this.member.id)
          );
        } else {
          this.likesService.likeIds.update((ids) => [...ids, this.member.id]);
        }
      },
    });
  }
  onRouteParamsChange() {
    const user = this.accountService.currentUser();
    if (!user) return;
    if (
      this.messageService.hubConnection?.state ===
        HubConnectionState.Connected &&
      this.activateTab?.heading === 'Messages'
    ) {
      this.messageService.hubConnection.stop().then(() => {
        this.messageService.createHubConnection(user, this.member.username);
      });
    }
  }

  OnTabActivated(data: TabDirective) {
    this.activateTab = data;
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { tab: this.activateTab.heading },
      queryParamsHandling: 'merge',
    });
    if (this.activateTab.heading === 'Messages' && this.member) {
      const user = this.accountService.currentUser();
      if (!user) return;
      this.messageService.createHubConnection(user, this.member.username);
    } else {
      this.messageService.stopHubConnection();
    }
  }

  ngOnDestroy(): void {
    this.messageService.stopHubConnection();
  }

  // loadMember() {
  //   console.log('load Members');
  //   const username = this.route.snapshot.paramMap.get('username');
  //   if (!username) return;

  //   this.memberService.getMember(username).subscribe({
  //     next: (member) => (this.member = member),
  //   });
  // }
}
