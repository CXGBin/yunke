import React, { useEffect, useState } from 'react';
import { PageContainer } from '@ant-design/pro-components';
import { Card, Row, Col, Spin, Empty, Statistic, message } from 'antd';
import {
  CheckCircleOutlined,
  TeamOutlined,
  StarOutlined,
  DollarOutlined,
  RiseOutlined,
  FallOutlined,
} from '@ant-design/icons';
import { getStatisticsOverview } from '@/services/statistics';

const Overview: React.FC = () => {
  const [data, setData] = useState<API.StatisticsData | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string>();

  useEffect(() => {
    getStatisticsOverview()
      .then(setData)
      .catch((e) => setError(e?.message || '获取数据失败'))
      .finally(() => setLoading(false));
  }, []);

  if (loading) {
    return (
      <PageContainer>
        <div style={{ textAlign: 'center', padding: '80px 0' }}>
          <Spin size="large" tip="加载中..." />
        </div>
      </PageContainer>
    );
  }

  if (error) {
    return (
      <PageContainer>
        <Empty description={error} />
      </PageContainer>
    );
  }

  const stats = data || {};

  return (
    <PageContainer>
      <Row gutter={[16, 16]}>
        <Col xs={24} sm={12} lg={6}>
          <Card bordered={false}>
            <Statistic
              title="出勤率"
              value={stats.attendanceRate || 0}
              precision={1}
              suffix="%"
              prefix={<CheckCircleOutlined style={{ color: '#52c41a' }} />}
            />
          </Card>
        </Col>
        <Col xs={24} sm={12} lg={6}>
          <Card bordered={false}>
            <Statistic
              title="选课率"
              value={stats.enrollmentRate || 0}
              precision={1}
              suffix="%"
              prefix={<TeamOutlined style={{ color: '#1677ff' }} />}
            />
          </Card>
        </Col>
        <Col xs={24} sm={12} lg={6}>
          <Card bordered={false}>
            <Statistic
              title="满意度评分"
              value={stats.satisfactionScore || 0}
              precision={1}
              suffix="/ 5.0"
              prefix={<StarOutlined style={{ color: '#faad14' }} />}
            />
          </Card>
        </Col>
        <Col xs={24} sm={12} lg={6}>
          <Card bordered={false}>
            <Statistic
              title="营收趋势"
              value={stats.revenueList?.length || 0}
              suffix=" 条数据"
              prefix={<DollarOutlined style={{ color: '#f5222d' }} />}
            />
          </Card>
        </Col>
      </Row>

      <Row gutter={[16, 16]} style={{ marginTop: 16 }}>
        <Col xs={24} lg={12}>
          <Card title="营收趋势" bordered={false}>
            <div style={{ textAlign: 'center', color: '#999', padding: '60px 0' }}>
              图表区域 — 接入 ECharts/Chart.js 后展示营收趋势图
            </div>
          </Card>
        </Col>
        <Col xs={24} lg={12}>
          <Card title="机构增长趋势" bordered={false}>
            <div style={{ textAlign: 'center', color: '#999', padding: '60px 0' }}>
              图表区域 — 接入 ECharts/Chart.js 后展示机构增长图
            </div>
          </Card>
        </Col>
      </Row>

      <Row gutter={[16, 16]} style={{ marginTop: 16 }}>
        <Col xs={24} lg={8}>
          <Card title="出勤分布" bordered={false}>
            <div style={{ textAlign: 'center', color: '#999', padding: '60px 0' }}>
              图表占位
            </div>
          </Card>
        </Col>
        <Col xs={24} lg={8}>
          <Card title="选课分布" bordered={false}>
            <div style={{ textAlign: 'center', color: '#999', padding: '60px 0' }}>
              图表占位
            </div>
          </Card>
        </Col>
        <Col xs={24} lg={8}>
          <Card title="满意度分布" bordered={false}>
            <div style={{ textAlign: 'center', color: '#999', padding: '60px 0' }}>
              图表占位
            </div>
          </Card>
        </Col>
      </Row>
    </PageContainer>
  );
};

export default Overview;
